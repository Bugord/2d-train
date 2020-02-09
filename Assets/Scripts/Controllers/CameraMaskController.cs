using Assets.Scripts.Services;
using UnityEngine;

namespace Controllers
{
    public class CameraMaskController : MonoBehaviour
    {
        private Animator _animator;
        private UIService _uiService;
        private readonly int _isInGame = Animator.StringToHash("IsInGame");
        private readonly int _isInStore = Animator.StringToHash("IsInStore");

        void Awake()
        {
            _animator = GetComponent<Animator>();
            _uiService = ServiceLocator.GetService<UIService>();
            _uiService.SetActiveInGameUI += UpdateIsInGameState;
            _uiService.SetActiveStoreMenu += UpdateIsInStoreState;
        }

        private void UpdateIsInGameState(bool isInGame)
        {
            _animator.SetBool(_isInGame, isInGame);
        }

        private void UpdateIsInStoreState(bool isInStore)
        {
            _animator.SetBool(_isInStore, isInStore);
        }
    }
}
