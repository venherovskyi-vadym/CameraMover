public interface ICameraStrategy
{
    void Init(float time);
    void Update(float time, float timeDelta);
    void Finish();
}