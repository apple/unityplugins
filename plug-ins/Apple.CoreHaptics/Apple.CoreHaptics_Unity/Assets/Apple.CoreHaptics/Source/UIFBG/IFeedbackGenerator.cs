namespace Apple.UIFBG
{
	public interface IFeedbackGenerator
	{
		void Prepare();
		void Destroy();
		void Trigger();
	}
}
