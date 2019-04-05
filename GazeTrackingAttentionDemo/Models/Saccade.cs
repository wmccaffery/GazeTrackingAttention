using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazeTrackingAttentionDemo.Models
{
	class Saccade
	{
		Fixation p1;
		Fixation p2;

		float duration;
		float isRegression;
	}
}
