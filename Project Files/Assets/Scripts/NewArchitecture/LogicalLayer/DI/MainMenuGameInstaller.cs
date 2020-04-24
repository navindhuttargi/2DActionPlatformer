using ActionPlatformer.UI;
using Zenject;
namespace ActionPlatformer
{
	public class MainMenuGameInstaller : MonoInstaller<MainMenuGameInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<IControllerFactory>().To<ControllerFactory>().AsSingle();
			Container.Bind<IGameUI>().To<UIManager>().AsSingle();
			Container.Bind<IGameController>().To<GameController>().AsSingle();
		}
	}
}
