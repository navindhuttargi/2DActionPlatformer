namespace ActionPlatformer
{
	public abstract class State
	{
		private uint id;
		public State(uint _id)
		{
			id = _id;
		}
		public uint ID
		{
			get { return id; }
			set { id = value; }
		}
		public virtual void OnEnter() { }
		public virtual void OnUpdate() { }
		public virtual void OnExit() { }
	}

}
