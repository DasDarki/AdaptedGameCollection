using AdaptedGameCollection.Server;


if (AGCServer.Run())
{
    AGCServer.Instance.BlockThread();
    AGCServer.Instance.Stop();
}