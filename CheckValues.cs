using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IssleduemSmetanu
{
    class CheckValues
    {
        public static void checkValue(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Text == "")
                    return;

                double? textBoxValue = checkDouble(textBox.Text);

                if (textBoxValue == null)
                {
                    callDialog("Введённые данные некорректны\nЗначение должно быть числом", DialogType.Error);
                    textBox.Focus();
                    return;
                }
                else
                    textBox.Text = textBoxValue.ToString();

                switch (textBox.Name)
                {
                    case "widthTextBox":
                        checkPositive((double)textBoxValue, "Ширина", sender);
                        break;
                    case "depthTextBox":
                        checkPositive((double)textBoxValue, "Глубина", sender);
                        break;
                    case "lenghtTextBox":
                        checkPositive((double)textBoxValue, "Длина", sender);
                        break;
                    case "densityTextBox":
                        checkPositive((double)textBoxValue, "Плотность", sender);
                        break;
                    case "specificHeatCapacityTextBox":
                        checkPositive((double)textBoxValue, "Удельная теплоёмкость", sender);
                        break;
                    case "meltingPointTextBox":
                        CheckValues.checkCelsius((double)textBoxValue, "Температура плавления", sender);
                        break;
                    case "capSpeedTextBox":
                        checkPositive((double)textBoxValue, "Скорость крышки", sender);
                        break;
                    case "capTempTextBox":
                        checkCelsius((double)textBoxValue, "Температура крышки", sender);
                        break;
                    case "viscosityTextBox":

                        break;
                    case "tempRatioTextBox":

                        break;
                    case "castingTempTextBox":
                        checkCelsius((double)textBoxValue, "Температура приведения", sender);
                        break;
                    case "timeConstTextBox":

                        break;
                    case "viscosityAnomalyTextBox":

                        break;
                    case "heatTransferRatioTextBox":

                        break;
                    case "tableStepTextBox":

                        break;
                    case "graphStepTextBox":

                        break;
                    default:
                        break;
                }

            }

            return;
        }

        public static double? checkDouble(string input)
        {
            return double.TryParse(input, NumberStyles.Float, new CultureInfo("ru-RU"), out double result) ? result : (double?)null;
        }

        public static void checkPositive(double value, string parameterName, object sender)
        {
            if (value <= 0)
            {
                callDialog($"Значение параметра {parameterName} должно быть положительным", DialogType.Error);
                (sender as TextBox).Focus();
                return;
            }
        }

        public static void checkCelsius(double value, string parameterName, object sender)
        {
            if (value < -273)
            {
                callDialog($"Значение параметра {parameterName} должно быть больше -273°С", DialogType.Error);
                (sender as TextBox).Focus();
                return;
            }
        }

        private static string callDialog(string message, DialogType type)
        {
            Dialog error = new Dialog(message, type);
            error.ShowDialog();
            return error.ActionCode;
        }
    }
}
