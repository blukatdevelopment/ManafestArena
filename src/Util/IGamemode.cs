public interface IGamemode {
    Actor GetPlayer();
    string GetObjectiveText();
    void HandleEvent(SessionEvent evt);
    void Init(string[] argv);
    bool GameOver();
    bool Victory();
    void Update(float delta);
}