using Assets.Scripts.Services;
using UnityEngine;

namespace Controllers
{
    public class CameraMaskController : MonoBehaviour
    {
        private Animator _animator;
        private UIService _uiService;
        private readonly int _isInGame = Animator.StringToHash("IsInGame");

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _uiService = ServiceLocator.GetService<UIService>();
            _uiService.SetActiveInGameUI += UpdateAnimatorState;
        }

        private void UpdateAnimatorState(bool isInGame)
        {
            _animator.SetBool(_isInGame, isInGame);
        }
    }
}
