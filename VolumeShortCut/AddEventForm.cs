using System.ComponentModel;
using System.Windows.Forms;

namespace VolumeShortCut
{
    public class AddEventForm : Form
    {

        private event FormShowingEventHandler formShowing;
        private event FormHidingEventHandler formHiding;

        public event FormShowingEventHandler FormShowing
        {
            add { formShowing += value; }
            remove { formShowing -= value; }
        }

        public event FormHidingEventHandler FormHiding
        {
            add { formHiding += value; }
            remove { formHiding -= value; }
        }

        public AddEventForm()
        {
            formShowing += (sender, e) => { };
            formHiding += (sender, e) => { };
        }

        protected override void SetVisibleCore(bool value)
        {
            var current = Visible;
            var toBe = value;

            if (current == toBe)
            {
                base.SetVisibleCore(toBe);
                return;
            }

            if (toBe && !current)
            {
                var e = new FormShowingEventArgs();
                formShowing(this, e);
                if (e.Cancel)
                {
                    toBe = current;
                }
            }
            if (!toBe && current)
            {
                var e = new FormHidingEventArgs();
                formHiding(this, e);
                if (e.Cancel)
                {
                    toBe = current;
                }
            }
            base.SetVisibleCore(toBe);
        }
        public delegate void FormShowingEventHandler(object sender, FormShowingEventArgs e);
        public delegate void FormHidingEventHandler(object sender, FormHidingEventArgs e);

        public class FormShowingEventArgs : CancelEventArgs { }
        public class FormHidingEventArgs : CancelEventArgs { }
    }
}
