using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private float _minTimeToSpeak;
    [SerializeField] private float _maxTimeToSpeak;
    [SerializeField] private List<SoundLine> _possibleSoundLines;
    private Queue<SoundLine> _currentSoundLines = new Queue<SoundLine>();
    private Coroutine _speakingCoroutine;
    private bool _canSpeak = true;
    private AudioSource _audioSource;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _speakingCoroutine = StartCoroutine(Speak());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(PlayerController controller)
    {
        QuestManager.Instance.QuestGiverTriggered();
    }

    private IEnumerator Speak()
    {
        while(_canSpeak)
        {
            BuildSoundLines();
            while(_currentSoundLines.TryDequeue(out SoundLine currentLine))
            {
                _audioSource.PlayOneShot(currentLine.clip);
                yield return new WaitUntil(() => !_audioSource.isPlaying);
            }
            yield return new WaitForSeconds(Random.Range(_minTimeToSpeak, _maxTimeToSpeak));
        }
    }

    private void BuildSoundLines()
    {
        var tempLines = new List<SoundLine>(_possibleSoundLines);
        var length = Random.Range(1, 4);
        for (int i = 0; i < length; i++)
        {
            var lineToAnalyse = tempLines[Random.Range(0, tempLines.Count)];
            tempLines.Remove(lineToAnalyse);

            if(_currentSoundLines.Any(sound => lineToAnalyse.conflictsWithLines.Contains(sound.name)))
            {
                continue;
            }

            _currentSoundLines.Enqueue(lineToAnalyse);
        }
    }

    [System.Serializable]
    private class SoundLine
    {
        public string name;
        public AudioClip clip;
        public List<string> conflictsWithLines;

        public override bool Equals(object obj)
        {
            if(object.ReferenceEquals(this, obj)) return true;
            var sound = obj as SoundLine;
            if(sound is null) return false;
            return this.name == sound.name;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }
    }
}
