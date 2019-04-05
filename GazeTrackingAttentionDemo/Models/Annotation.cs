using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//binding between paragraph text and eye data
namespace GazeTrackingAttentionDemo.Models
{
	class DataParagraph
	{
		//start and end of data
		//store top and bottom y coordinates
		float minY;
		float maxY;

		//metrics
		int interest;
		int attentiveness;
		int effort;

		String comments;

	}
}
