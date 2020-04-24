using ActionPlatformer.Gameplay;
using ActionPlatformer.UI;
using Zenject;
namespace ActionPlatformer
{
	public class GameplayInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<IControllerFactory>().To<InGameControllerFactory>().AsSingle();
			Container.Bind<IGameUI>().To<GameplayUIManager>().AsSingle();
			Container.Bind<IGameController>().To<GameController>().AsSingle();
		}
	}
}
