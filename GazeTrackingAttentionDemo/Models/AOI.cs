﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace GazeTrackingAttentionDemo.Models
{
	public class AOI
	{
		string _name;
		int _interest;
		int _attentiveness;
		int _effort;
		String _comments;
		public Polygon p;
		//data

		public AOI()
		{

		}

		public string Name { get => _name; set => _name = value; }
		public int Interest { get => _interest; set => _interest = value; }
		public int Attentiveness { get => _attentiveness; set => _attentiveness = value; }
		public int Effort { get => _effort; set => _effort = value; }
		public string Comments { get => _comments; set => _comments = value; }


		public bool checkOverlap()
		{
			return false;
		}
	}
}
