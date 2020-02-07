using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace csharp_calculator
{
    public partial class calculator : Form
    {
        public string calculator_text { get; private set; }
        private List<(string numstr, CALCULATOR_OPERATORS op)> calculator_history = new List<(string, CALCULATOR_OPERATORS)>();
        private bool dot_added = false;
        private bool input_since_operator = false;
        private bool operator_set = false;

        enum CALCULATOR_OPERATORS
        {
            CALCULATOR_NONE,
            CALCULATOR_DIVIDE,
            CALCULATOR_MULTIPLY,
            CALCULATOR_MINUS,
            CALCULATOR_PLUS
        };

        CALCULATOR_OPERATORS calculator_operator = CALCULATOR_OPERATORS.CALCULATOR_NONE;

        private char operator_to_char(CALCULATOR_OPERATORS op)
        {
            switch (op)
            {
                case CALCULATOR_OPERATORS.CALCULATOR_DIVIDE:
                    return '/';
                case CALCULATOR_OPERATORS.CALCULATOR_MINUS:
                    return '-';
                case CALCULATOR_OPERATORS.CALCULATOR_MULTIPLY:
                    return '*';
                default:
                    break;
            }

            return '+';
        }

        private void update_text()
        {
            // format to see thousands
            var num = Decimal.Parse(calculator_text, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);
            var str = String.Format(CultureInfo.InvariantCulture, (dot_added && calculator_text[calculator_text.Length - 1] != '.') ? "{0:n" + (calculator_text.Length - (calculator_text.IndexOf('.') + 1)).ToString() + "}" : "{0:n0}", num);

            if (dot_added && calculator_text[calculator_text.Length - 1] == '.') str += '.';
     
            label1.Text = str;
            label1.Update();
        }

        private void update_history()
        {
            if (calculator_history.Count == 0)
            {
                label2.Text = "";
                label2.Update();

                return;
            }

            string history = "";

            bool first = true;

            foreach (var item in calculator_history)
            {
                if (!first) history += ' ';

                first = false;

                history += item.numstr + ' ' + operator_to_char(item.op);
            }

            label2.Text = history;
            label2.Update();
        }

        private void calculate(bool clear_history)
        {
            if (calculator_history.Count == 0) return;

            var num = Convert.ToDouble(Decimal.Parse(calculator_text, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));
            var last_op = calculator_history[calculator_history.Count - 1];
            var last_op_double = Convert.ToDouble(Decimal.Parse(last_op.numstr, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture));

            switch (last_op.op)
            {
                case CALCULATOR_OPERATORS.CALCULATOR_DIVIDE:
                    num /= last_op_double;
                    break;
                case CALCULATOR_OPERATORS.CALCULATOR_MINUS:
                    num -= last_op_double;
                    break;
                case CALCULATOR_OPERATORS.CALCULATOR_MULTIPLY:
                    num *= last_op_double;
                    break;
                default:
                    num += last_op_double;
                    break;
            }

            if (clear_history)
            {
                calculator_history.Clear();

                label2.Text = "";
                label2.Update();
            }

            calculator_text = num.ToString();
            dot_added = calculator_text.Length > 0 && calculator_text.IndexOf('.') > 0;
            operator_set = false;
            input_since_operator = false;

            update_text();
        }

        private void set_operator(CALCULATOR_OPERATORS op)
        {
            calculator_operator = op;

            if (input_since_operator) calculate(false);
            if (operator_set) calculator_history.RemoveAt(calculator_history.Count - 1);

            calculator_history.Add((calculator_text, op));

            update_history();

            operator_set = true;

            input_since_operator = false;
        }

        private void reset()
        {
            calculator_text = "0";

            update_text();

            dot_added = false;

            calculator_operator = CALCULATOR_OPERATORS.CALCULATOR_NONE;

            calculator_history.Clear();
        }

        private void add_number(int num)
        {
            if (num == 0 && calculator_text.Length == 1 && calculator_text.Equals("0")) return;
            if (num != 0 && calculator_text.Length == 1 && calculator_text.Equals("0")) calculator_text = "";

            if (!input_since_operator/* && calculator_operator != CALCULATOR_OPERATORS.CALCULATOR_NONE*/)
            {
                calculator_text = num.ToString();

                operator_set = false;
                dot_added = false;
            }
            else
            {
                calculator_text += num.ToString();
            }

            update_text();

            input_since_operator = true;
        }

        private void add_dot()
        {
            if (dot_added) return;
            if (!input_since_operator && calculator_operator != CALCULATOR_OPERATORS.CALCULATOR_NONE)
            {
                calculator_text = "0.";

                label1.Text = calculator_text;
                label1.Update();

                input_since_operator = true;
                operator_set = false;

                return;
            }

            calculator_text += '.';

            dot_added = true;

            update_text();
        }

        private void negate_number()
        {
            var num = Decimal.Parse(calculator_text, NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture);

            if (num == 0) return;

            num *= -1;

            calculator_text = num.ToString();

            update_text();

            if (!input_since_operator && calculator_operator != CALCULATOR_OPERATORS.CALCULATOR_NONE)
            {
                input_since_operator = true;
                operator_set = false;
            }
        }

        private void remove_number()
        {
            if (calculator_text.Length == 1)
            {
                calculator_text = "0";

                dot_added = false;

                update_text();

                return;
            }

            if (dot_added && calculator_text[calculator_text.Length - 1] == '.') dot_added = false;

            calculator_text = calculator_text.Remove(calculator_text.Length - 1, 1);

            update_text();
        }

        public calculator()
        {
            InitializeComponent();

            calculator_text = "0";

            KeyPreview = true;
            KeyPress += new KeyPressEventHandler(keypressed);
        }

        // all the numbers (0-9)
        private void button7_Click(object sender, EventArgs e)
        {
            add_number(1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            add_number(7);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            add_number(9);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            add_number(8);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            add_number(4);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            add_number(5);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            add_number(6);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            add_number(2);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            add_number(3);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            add_number(0);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            negate_number();
        }

        const int WM_KEYDOWN = 0x100;
        const int WM_SYSKEYDOWN = 0x104;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                if (keyData >= Keys.D0 && keyData <= Keys.D9) add_number((int)(keyData - Keys.D0));
                if (keyData == Keys.Back) remove_number();
                //if (keyData == Keys.OemPeriod) add_dot();
                if (keyData == Keys.Enter)
                {
                    calculate(true);
                    //if (calculator_operator == CALCULATOR_OPERATORS.CALCULATOR_NONE) return true;
                    return true;
                }
                //if (keyData == Keys.OemMinus) set_operator(CALCULATOR_OPERATORS.CALCULATOR_MINUS);
                //if (keyData == Keys.Oemplus) set_operator(CALCULATOR_OPERATORS.CALCULATOR_PLUS);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void keypressed(Object o, KeyPressEventArgs e)
        {
            if (e.KeyChar == '-') set_operator(CALCULATOR_OPERATORS.CALCULATOR_MINUS);
            if (e.KeyChar == '+') set_operator(CALCULATOR_OPERATORS.CALCULATOR_PLUS);
            if (e.KeyChar == '*') set_operator(CALCULATOR_OPERATORS.CALCULATOR_MULTIPLY);
            if (e.KeyChar == '/') set_operator(CALCULATOR_OPERATORS.CALCULATOR_DIVIDE);
            if (e.KeyChar == '.') add_dot();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            remove_number();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            add_dot();
        }

        private void button19_Click(object sender, EventArgs e)
        {
            set_operator(CALCULATOR_OPERATORS.CALCULATOR_PLUS);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            calculate(true);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            set_operator(CALCULATOR_OPERATORS.CALCULATOR_MINUS);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            set_operator(CALCULATOR_OPERATORS.CALCULATOR_MULTIPLY);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            set_operator(CALCULATOR_OPERATORS.CALCULATOR_DIVIDE);
        }
    }
}