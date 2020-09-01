using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Sudoku_V2._0
{
    public partial class Sudoku : Form
    {

        #region Variables

        private int[,] _oSudoku = null;
        private int[,] _oSudokuEspejo = null;

        #endregion Variables

        private void CargarSudoku()
        {
            try
            {
                _oSudoku = new int[9, 9];

                if (File.Exists(@"C:\Sudoku\Sudoku.txt"))
                {
                    var strSudoku = File.ReadAllText(@"C:\Sudoku\Sudoku.txt");

                    strSudoku = strSudoku.Replace(" ", "");

                    var strSudokuValues = strSudoku.Split('|');


                    Stack<int> stkPila = new Stack<int>();
                    Stack<int> stkPilaReversa = new Stack<int>();

                    foreach (var row in strSudokuValues)
                    {
                        if (row.Length > 0)
                        {
                            stkPilaReversa.Push(Convert.ToInt32(row));
                        }
                    }

                    int intRows = stkPilaReversa.Count;
                    for (int i = 0; i < intRows; i++ )
                    {
                        stkPila.Push(stkPilaReversa.Pop());
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            _oSudoku[j,i] = stkPila.Pop();
                        }
                    }

                    RandomizarSudoku();
                    PrepararParaJugar();
                    bgwValidador.RunWorkerAsync();
                }
                else
                {
                    throw new Exception("Archivo de inicio no encontrado.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar Sudoku: " + ex.Message);
            }
        }

        private void PrepararParaJugar()
        {
            try
            {
                Stack<int> stkNumerosQuitar = new Stack<int>();
                List<int> lstCoordenadas = new List<int>();
                int[,] intMatrizTemp = new int[9, 9];

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        intMatrizTemp[j,i] = _oSudoku[j, i];
                    }
                }

                Random rmd = new Random();

                int intNumerosQuitar = rmd.Next(20, 40);

                for (int i = 0; i < intNumerosQuitar; i++)
                {
                    int intValorX = rmd.Next(1, 9);
                    int intValorY = rmd.Next(1, 9);

                    if (lstCoordenadas.Contains(Convert.ToInt32(intValorX.ToString() + intValorY.ToString())))
                    {
                        i--;
                        continue;
                    }

                    lstCoordenadas.Add(Convert.ToInt32(intValorX.ToString() + intValorY.ToString()));
                }

                foreach (var row in lstCoordenadas)
                {
                    int intValorX = Convert.ToInt32(row.ToString()[0].ToString());
                    int intValorY = Convert.ToInt32(row.ToString()[1].ToString());

                    intMatrizTemp[intValorY, intValorX] = 0;
                }


                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        var oColl = this.Controls;
                        Control[] colection = oColl.Find("e" + i.ToString() + j.ToString(), false);
                        var stValor = (TextBox)colection[0];

                        stValor.Enabled = true;

                        stValor.Text = intMatrizTemp[j, i].ToString();

                        if (intMatrizTemp[j, i] != 0)
                        {
                            stValor.Enabled = false;
                        }

                        if (intMatrizTemp[j, i] == 0)
                            stValor.Text = "";
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error al preparar para jugar: " + ex.Message);
            }
        }

        private void RandomizarSudoku()
        {
            try
            {
                Random rmd = new Random();
                Stack<int> stkNumerosCambio = new Stack<int>();

                int intNumerosACambiar = rmd.Next(1, 30);
                int intGiros = rmd.Next(4);
                int intNumeroAnterior = 0;

                for (int i = 0; i < intNumerosACambiar; i++)
                {
                    int intValorIngresar = rmd.Next(1, 10);

                    if (intValorIngresar == intNumeroAnterior)
                    {
                        i--;
                        continue;
                    }

                    stkNumerosCambio.Push(intValorIngresar);
                    intNumeroAnterior = intValorIngresar;
                }

                intNumerosACambiar = stkNumerosCambio.Count;

                int intNumeroX = 0;
                int intNumeroY = 0;

                for (int i = 0; i < intNumerosACambiar; i++)
                {
                    if (stkNumerosCambio.Count == 0)
                        break;

                    if (intNumeroX == 0)
                        intNumeroX = stkNumerosCambio.Pop();

                    if (intNumeroY == 0)
                        intNumeroY = stkNumerosCambio.Pop();

                    CambiarNumeros(intNumeroX, intNumeroY);

                    intNumeroX = intNumeroY;
                    intNumeroY = 0;
                }

                for (int i = 0; i < intGiros; i++)
                {

                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error al randomizar el Sudoku: " + ex.Message);
            }
        }

        private void CambiarNumeros(int intNumeroX, int intNumeroY)
        {
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (_oSudoku[j, i] == intNumeroX)
                            _oSudoku[j, i] = 0;
                    }
                }


                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (_oSudoku[j, i] == intNumeroY)
                            _oSudoku[j, i] = intNumeroX;
                    }
                }


                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (_oSudoku[j, i] == 0)
                            _oSudoku[j, i] = intNumeroY;
                    }
                }

            }
            catch(Exception ex)
            {
                throw new Exception("Error al cambiar numeros: " + ex.Message);
            }
        }

        private void LlenarSudoku(char chValor)
        {
            try
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        var oColl = this.Controls;
                        Control[] colection = oColl.Find("e" + i.ToString() + j.ToString(), false);
                        var stValor = (TextBox)colection[0];

                        stValor.Text = chValor.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en llenar sudoku: " + ex.Message);
            }
        }

        public Sudoku()
        {
            InitializeComponent();

            try
            {
                CargarSudoku();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                bgwValidador.CancelAsync();

                while (!bgwValidador.CancellationPending)
                {

                }

                RandomizarSudoku();
                PrepararParaJugar();

                bgwValidador.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void e88_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void bgwValidador_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                _oSudokuEspejo = new int[9, 9];
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        _oSudokuEspejo[j, i] = _oSudoku[j, i];
                    }
                }

                int intErrores = 0;

                bool booTerminado = false;
                while (!booTerminado)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            var oColl = this.Controls;
                            Control[] colection = oColl.Find("e" + i.ToString() + j.ToString(), false);
                            var stValor = (TextBox)colection[0];

                            if (!string.IsNullOrEmpty(stValor.Text))
                                stValor.Invoke((MethodInvoker)delegate { stValor.ForeColor = _oSudoku[j, i].ToString() == stValor.Text ? Color.Green : Color.Red; ; });

                        }
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            var oColl = this.Controls;
                            Control[] colection = oColl.Find("e" + i.ToString() + j.ToString(), false);
                            var stValor = (TextBox)colection[0];

                            if (string.IsNullOrEmpty(stValor.Text))
                                continue;

                            if (Convert.ToInt32(stValor.Text) != _oSudokuEspejo[j, i])
                            {
                                if (Convert.ToInt32(stValor.Text) == _oSudoku[j, i])
                                {
                                    _oSudokuEspejo[j, i] = Convert.ToInt32(stValor.Text);
                                    continue;
                                }

                                intErrores++;
                                _oSudokuEspejo[j, i] = Convert.ToInt32(stValor.Text);
                            }
                        }
                    }

                    if (intErrores > 0)
                    {
                        lblErrores.Invoke((MethodInvoker)delegate { lblErrores.Text = "Errores: " + intErrores.ToString(); });
                    }

                    booTerminado = true;
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            var oColl = this.Controls;
                            Control[] colection = oColl.Find("e" + i.ToString() + j.ToString(), false);
                            var stValor = (TextBox)colection[0];

                            if (string.IsNullOrEmpty(stValor.Text))
                                booTerminado = false;
                            if (stValor.ForeColor == Color.Red)
                                booTerminado = false;

                            if (!booTerminado)
                                break;
                        }
                        if (!booTerminado)
                            break;
                    }
                }

                MessageBox.Show("Terminado, felicidades. Errores cometidos: " + intErrores.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception("Error en el Do-Work: " + ex.Message);
            }
        }
    }
}
