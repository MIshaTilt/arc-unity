# 📦 Настройка PocketBase для Unity-проекта

## 1. Запуск PocketBase сервера

### Вариант А: Локальный запуск (рекомендуется для разработки)

1. Скачайте последнюю версию PocketBase с https://pocketbase.io/docs/
2. Распакуйте архив в удобное место (например `C:\pocketbase\`)
3. Запустите сервер:
   ```bash
   cd C:\pocketbase
   pocketbase serve
   ```
4. Откройте админку: http://127.0.0.1:8090/_/
5. Создайте администратора (email + password)

### Вариант Б: Docker

```bash
docker run -d \
  -p 8090:8090 \
  -v ./pb_data:/pb_data \
  --name pocketbase \
  ghcr.io/muchobien/pocketbase:latest serve
```

## 2. Настройка коллекций в админке

### 2.1. Коллекция `game_saves`

1. В админке PocketBase перейдите в **Settings → Collections**
2. Нажмите **New collection**
3. Название: `game_saves`
4. Добавьте следующие поля:

| Field name | Type | Required | Notes |
|---|---|---|---|
| `sceneName` | text | ❌ | |
| `timestamp` | text | ❌ | |
| `playerPosition` | json | ❌ | `{positionX, positionY, positionZ, rotationY}` |
| `playerState` | json | ❌ | `{currentHealth, maxHealth, magicCooldownTimer, physicalCooldownTimer, isDead}` |
| `enemies` | json | ❌ | Массив: `[{id, enemyType, positionX, positionY, positionZ, rotationY, currentHealth, maxHealth, isAlive}]` |

> **Примечание:** Поле `id` — встроенный primary key PocketBase, не нужно создавать вручную.

5. **Rules** (Permissions):
   - List: `true`
   - View: `true`
   - Create: `true`
   - Update: `true`
   - Delete: `true`

> ⚠️ Для production настройте авторизацию и ограничьте правила!

### 2.2. Коллекция `users` (для авторизации)

Создаётся автоматически. Создайте пользователя:
- Email: `admin@example.com`
- Password: `change_me`

## 3. Настройка в Unity

1. Откройте сцену `GameplayScene`
2. Найдите объект с `GameBootstrapper`
3. В инспекторе:
   - ✅ Включите **Use PocketBase**
   - Заполните **Pocket Base Config**:
     - **Base Url**: `http://localhost:8090`
     - **Admin Email**: `admin@example.com`
     - **Admin Password**: `change_me`
     - **Saves Collection**: `game_saves`

4. **Enemy Save IDs** — задайте уникальные ID для каждого врага на сцене:
   - `enemy_melee_01`
   - `enemy_ranged_01`
   - и т.д.

## 4. Тестирование

1. Запустите игру
2. Передвиньте игрока, нанесите урон
3. Откройте паузу (Esc) → **Save**
4. Проверьте админку PocketBase — должна появиться запись в `game_saves`
5. Перезагрузите сцену → **Load** — позиция и HP должны восстановиться

## 5. Архитектура системы сохранения

```
┌─────────────────────────────────────────────────────────────┐
│                        Unity Client                         │
├─────────────────────────────────────────────────────────────┤
│  PauseMenuController                                        │
│       ↓                                                     │
│  ISaveService (PocketBaseSaveService)                       │
│       ↓                                                     │
│  ISaveLoadInteractor (SaveLoadInteractor)                   │
│       ↓                                                     │
│  IRepository<GameSaveData> (PocketBaseRepository)           │
│       ↓                                                     │
│  HTTP REST API → PocketBase Server                          │
└─────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────┐
│                   IEntitySaveable                           │
│  - PlayerMovement : IEntitySaveable                         │
│  - EnemyAI : IEntitySaveable                                │
└─────────────────────────────────────────────────────────────┘
```

## 6. Структура данных

### GameSaveData (полный снимок сохранения)
```json
{
  "id": "quicksave",
  "sceneName": "GameplayScene",
  "timestamp": "2026-04-13 15:30:00",
  "playerPosition": {
    "positionX": 10.5,
    "positionY": 1.2,
    "positionZ": -3.7,
    "rotationY": 180.0
  },
  "playerState": {
    "currentHealth": 75.0,
    "maxHealth": 100.0,
    "magicCooldownTimer": 2.3,
    "physicalCooldownTimer": 0.0,
    "isDead": false
  },
  "enemies": [
    {
      "id": "enemy_melee_01",
      "enemyType": "MeleeWalk",
      "positionX": 5.0,
      "positionY": 0.0,
      "positionZ": 8.0,
      "rotationY": 45.0,
      "currentHealth": 30.0,
      "maxHealth": 100.0,
      "isAlive": true
    }
  ]
}
```

## 7. Переключение на production

1. Задеплойте PocketBase на сервер (VPS, Docker)
2. Обновите `Base Url` в конфиге
3. Создайте реальных пользователей с токенами
4. Включите авторизацию в `PocketBaseRepository.AuthenticateAsync()`
5. Настройте HTTPS

## 8. Миграция на PostgreSQL (если нужно)

Благодаря паттерну Repository, замена бэкенда не требует изменения бизнес-логики:

```csharp
// Было:
IRepository<GameSaveData> repo = new PocketBaseRepository<GameSaveData>(config);

// Стало:
IRepository<GameSaveData> repo = new PostgresRepository<GameSaveData>(connectionString);
```

Интерактор и сервис остаются без изменений!
