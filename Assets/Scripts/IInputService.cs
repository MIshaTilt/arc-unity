// IInputService.cs
using System;
using UnityEngine;

namespace Scripts.Services
{
    public interface IInputService
    {
        Vector2 MoveInput { get; }
        Vector2 LookInput { get; }
        bool IsSprinting { get; }
        
        event Action OnPhysicalAttack;
        event Action OnMagicAttack;
    }
}