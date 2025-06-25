using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField]
    private int _maxHp;
    private int _hp;

    public UnityEvent<int> Damaged;
    public UnityEvent Died;
    public UnityEvent phaseTransition;
    public UnityEvent phase1;
    public UnityEvent phase2;

    public int MaxHp => _maxHp;
    public int Hp
    {
        get => _hp;
        private set
        {
            var isDamage = value < _hp;
            _hp = Mathf.Clamp(value, 0, _maxHp);
            if (isDamage)
            {
                Damaged?.Invoke(_hp);
            }

            if (_hp > _maxHp / 2)
            {
                phase1?.Invoke();
            }

            if (_hp <= _maxHp/2)
            {
                phase2?.Invoke();
            }

            if(_hp == _maxHp/2)
            {
                phaseTransition?.Invoke();
            }

            if(_hp <=0)
            {
                Died?.Invoke();
            }
        }
    }

    private void Awake()
    {
        _hp = _maxHp;
    }

    public void Damage(int amount) => Hp -= amount;  
    public void Kill() => Hp = 0;
}
