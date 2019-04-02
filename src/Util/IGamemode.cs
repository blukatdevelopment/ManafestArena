interface IGamemode {
    Actor GetPlayer();
    string GetObjectiveText();
    void HandleEvent(SessionEvent evt);
    void Init(string[] argv);
}