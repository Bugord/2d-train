using Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class SoundSwitchButton : MonoBehaviour
    {
        private Image _image;
        private Button _button;

        private AudioService _audioService;

        private GameDataService _gameDataService;

        [SerializeField] private Sprite _on;

        [SerializeField] private Sprite _off;
        // Start is called before the first frame update
        void Awake()
        {
            _image = GetComponent<Image>();
            _button = GetComponent<Button>();
            _audioService = ServiceLocator.GetService<AudioService>();
            _gameDataService = ServiceLocator.GetService<GameDataService>();

            _audioService.volumeOn = _gameDataService.SoundOn;
            _image.sprite = _audioService.volumeOn ? _on : _off;
            _button.onClick.AddListener(SwitchVolume);
        }

        private void SwitchVolume()
        {
            _gameDataService.SoundOn = !_gameDataService.SoundOn;
            _audioService.volumeOn = _gameDataService.SoundOn;
            _image.sprite = _audioService.volumeOn ? _on : _off;
        }
    }
}
