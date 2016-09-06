using StaggeredGridView.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Model
{
    public class SampleItem : IStaggeredGridViewItem
    {
        public double Width { get; set; }

        public double Height { get; set; }

        public string ImageUrl { get; set; }

        public SampleItem(double w, double h, string img)
        {
            Height = h;
            Width = w;
            ImageUrl = "/Demo/" + img;
        }
    }
}