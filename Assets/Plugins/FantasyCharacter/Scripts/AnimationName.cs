public class AnimationName
{
	public const string Stand = "Stand";
	public const string Death = "Death";
	public const string Attack = "Attack";
	public const string Attacked = "Attacked";
	public const string Run = "Run";
	public const string Magic = "Magic";
	public const string Magic2 = "Magic2";
	public const string Ultimate = "Ultimate";
	public const string NewLife = "NewLife";
	public const string Victory = "Victory";
	public static string[] AllName = {
		Stand, Attack, Attacked, Run, Magic, Magic2, Ultimate, Death, NewLife, Victory
	};

	public static string[] breakName = {
		Attack, Run, Magic, Magic2, Ultimate
	};

	public static string[] showActionNameArr = {
		Attack, Run, Magic, Victory, Ultimate
	};

	public static string[] RandomActionNameArr = {
		Attack, Magic, Victory, Ultimate
	};
}
