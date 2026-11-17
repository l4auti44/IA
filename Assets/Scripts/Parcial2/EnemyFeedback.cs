using System.Collections;
using MoreMountains.Feedbacks;
using Patterns.Observer.Struct_EventManager;
using UnityEngine;

public class EnemyFeedback : MonoBehaviour
{
    [SerializeField] private MMF_Player warningFeedback;
    [SerializeField] private MMF_Player lostFeedback;
    private FOV _fov;
    private Coroutine _hideLostCoroutine;
    private void OnEnable()
    {
        _fov = GetComponent<FOV>();
        EventManager.Subscribe<PlayerFoundData>(SetWarningFeedback);
        _fov.OnPlayerDetected += SetLostFeedback;
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<PlayerFoundData>(SetWarningFeedback);
        _fov.OnPlayerDetected -= SetLostFeedback;
        StopLostFeedback();
    }

    private void SetWarningFeedback(PlayerFoundData data)
    {
        warningFeedback.gameObject.SetActive(true);
        warningFeedback.PlayFeedbacks();
        StartCoroutine(HideWarningFeedback(1));
    }

    private IEnumerator HideWarningFeedback(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        warningFeedback.ResetFeedbacks();
        warningFeedback.gameObject.SetActive(false);
    }

    private void SetLostFeedback(bool isDetected)
    {
        if (isDetected) 
        {
            StopLostFeedback();
            return;
        }

        PlayLostFeedback();
    }

    public void PlayLostFeedback()
    {
        if (!isActiveAndEnabled || lostFeedback == null) return;

        StopLostFeedback();
        lostFeedback.gameObject.SetActive(true);
        lostFeedback.PlayFeedbacks();
        _hideLostCoroutine = StartCoroutine(HideLostFeedback(1));
    }

    public void StopLostFeedback()
    {
        if (_hideLostCoroutine != null)
        {
            StopCoroutine(_hideLostCoroutine);
            _hideLostCoroutine = null;
        }

        if (lostFeedback == null) return;

        lostFeedback.ResetFeedbacks();
        if (lostFeedback.gameObject.activeSelf) lostFeedback.gameObject.SetActive(false);
    }

    private IEnumerator HideLostFeedback(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        _hideLostCoroutine = null;
        StopLostFeedback();
    }
}
