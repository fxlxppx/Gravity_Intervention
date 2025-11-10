using UnityEngine;
using UnityEngine.Audio; // Necessário para o AudioMixer
using UnityEngine.UI;     // Necessário para o Slider

public class VolumeControl : MonoBehaviour
{
    public AudioMixer mainMixer;
    public Slider volumeSlider;

    // O nome exato (string) do parâmetro que você criou no Mixer
    private const string MIXER_PARAM = "MasterVolume";

    // O nome da chave para salvar no PlayerPrefs
    private const string PREFS_KEY = "MasterVolume";

    private void Start()
    {
        // 1. Carrega o valor salvo (ou 0.5 se for a primeira vez)
        float savedVolume = PlayerPrefs.GetFloat(PREFS_KEY, 0.5f);

        // 2. Define o valor inicial do slider
        volumeSlider.value = savedVolume;

        // 3. Define o volume no mixer (para o som já começar correto)
        SetMixerVolume(savedVolume);

        // 4. AGORA, e SÓ AGORA, nós registramos o "listener".
        volumeSlider.onValueChanged.AddListener(SetVolumeAndSave);
    }

    // Esta função é chamada QUANDO O USUÁRIO mexe o slider
    public void SetVolumeAndSave(float volume)
    {
        // Define o volume no mixer
        SetMixerVolume(volume);

        // Salva a preferência do jogador
        PlayerPrefs.SetFloat(PREFS_KEY, volume);
    }

    // Função separada para apenas definir o áudio
    private void SetMixerVolume(float volume)
    {
        // O slider vai de 0.0001 a 1.
        // O mixer usa decibéis (escala logarítmica).
        float dbVolume = Mathf.Log10(volume) * 20f;
        mainMixer.SetFloat(MIXER_PARAM, dbVolume);
    }
}