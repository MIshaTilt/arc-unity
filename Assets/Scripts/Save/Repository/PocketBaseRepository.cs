using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Scripts.Save.Repository
{
    /// <summary>
    /// Репозиторий для работы с PocketBase через REST API.
    /// Реализует паттерн Repository — скывает детали HTTP-запросов.
    /// </summary>
    public class PocketBaseRepository<T> : IRepository<T> where T : class
    {
        private readonly string _baseUrl;
        private readonly string _collection;
        private string AuthToken { get; set; }

        public PocketBaseRepository(PocketBaseConfig config)
        {
            _baseUrl = config.BaseUrl.TrimEnd('/');
            _collection = config.SavesCollection;
        }

        /// <summary>
        /// Устанавливает токен авторизации (после логина).
        /// </summary>
        public void SetAuthToken(string token)
        {
            AuthToken = token;
        }

        public async Task<T> GetByIdAsync(string id)
        {
            string url = $"{_baseUrl}/api/collections/{_collection}/records/{id}";
            string response = await SendRequest(url, "GET");
            return response != null ? JsonUtility.FromJson<T>(response) : null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            string url = $"{_baseUrl}/api/collections/{_collection}/records?perPage=100";
            string response = await SendRequest(url, "GET");

            if (string.IsNullOrEmpty(response))
                return new List<T>();

            // PocketBase возвращает { "items": [...], "page": 1, ... }
            var wrapper = JsonUtility.FromJson<RecordListWrapper<T>>(response);
            return wrapper?.items ?? new List<T>();
        }

        public async Task<T> CreateAsync(T data)
        {
            string url = $"{_baseUrl}/api/collections/{_collection}/records";
            string json = JsonUtility.ToJson(data);
            string response = await SendRequest(url, "POST", json);
            return response != null ? JsonUtility.FromJson<T>(response) : null;
        }

        public async Task<T> UpdateAsync(string id, T data)
        {
            string url = $"{_baseUrl}/api/collections/{_collection}/records/{id}";
            string json = JsonUtility.ToJson(data);
            string response = await SendRequest(url, "PATCH", json);
            return response != null ? JsonUtility.FromJson<T>(response) : null;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            string url = $"{_baseUrl}/api/collections/{_collection}/records/{id}";
            string response = await SendRequest(url, "DELETE");
            return response != null;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            string url = $"{_baseUrl}/api/collections/{_collection}/records/{id}";
            string response = await SendRequest(url, "GET", suppressNotFoundLog: true);
            return !string.IsNullOrEmpty(response);
        }

        /// <summary>
        /// Авторизация в PocketBase (email + password).
        /// </summary>
        public async Task<bool> AuthenticateAsync(string email, string password)
        {
            string url = $"{_baseUrl}/api/collections/users/auth-with-password";
            string json = $"{{\"identity\":\"{email}\",\"password\":\"{password}\"}}";
            string response = await SendRequest(url, "POST", json);

            if (response != null)
            {
                // Парсим токен из ответа
                var authResponse = JsonUtility.FromJson<AuthResponse>(response);
                if (authResponse != null && !string.IsNullOrEmpty(authResponse.token))
                {
                    AuthToken = authResponse.token;
                    Debug.Log($"[PocketBase] Авторизация успешна. Токен: {AuthToken.Substring(0, 10)}...");
                    return true;
                }
            }

            Debug.LogError("[PocketBase] Ошибка авторизации.");
            return false;
        }

        #region HTTP Helper

        private async Task<string> SendRequest(string url, string method, string body = null, bool suppressNotFoundLog = false)
        {
            var tcs = new TaskCompletionSource<string>();

            using (var request = new UnityWebRequest(url, method))
            {
                request.SetRequestHeader("Content-Type", "application/json");

                if (!string.IsNullOrEmpty(AuthToken))
                {
                    request.SetRequestHeader("Authorization", AuthToken);
                }

                if (!string.IsNullOrEmpty(body))
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(body);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                }
                else if (method != "DELETE")
                {
                    request.downloadHandler = new DownloadHandlerBuffer();
                }

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    // 404 при ExistsAsync — не ошибка, а ожидаемый ответ
                    if (suppressNotFoundLog && request.responseCode == 404)
                    {
                        tcs.SetResult(null);
                        return await tcs.Task;
                    }

                    Debug.LogError($"[PocketBase] Ошибка запроса: {request.error}\nURL: {url}\nResponse: {request.downloadHandler?.text}");
                    tcs.SetResult(null);
                }
                else
                {
                    string responseText = request.downloadHandler?.text;
                    tcs.SetResult(responseText);
                }
            }

            return await tcs.Task;
        }

        #endregion

        #region DTO для ответов PocketBase

        [System.Serializable]
        private class AuthResponse
        {
            public string token;
        }

        [System.Serializable]
        private class RecordListWrapper<U>
        {
            public List<U> items;
        }

        #endregion
    }
}
