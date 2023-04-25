using Newtonsoft.Json;
using System.Numerics;

public class BaseOperation
{/* modified */
	public OperationEnum type { get; set; }
	public bool successful { get; set; }
	public Point target { get; set; }

	public BaseOperation(OperationEnum type, bool successful, Point target)
	{
		this.type = type;
		this.successful = successful;
		this.target = target;
	}

	public BaseOperation(){
		//Default Konstruktor
	}
}
