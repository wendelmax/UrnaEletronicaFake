using System;
using System.Windows;
using System.Windows.Controls;

namespace UrnaEletronicaFake.WPF.Views
{
    public partial class VotacaoWindow : Window
    {
        private string currentVote = "";
        private bool votacaoLiberada = false;
        private string eleitorAtual = "";

        public VotacaoWindow()
        {
            InitializeComponent();
            UpdateVotacaoStatus();
        }

        public void LiberarVotacao(string nomeEleitor)
        {
            eleitorAtual = nomeEleitor;
            votacaoLiberada = true;
            UpdateVotacaoStatus();
        }

        public void BloquearVotacao()
        {
            votacaoLiberada = false;
            eleitorAtual = "";
            currentVote = "";
            UpdateDisplay();
            UpdateCandidateInfo();
            UpdateConfirmButton();
            UpdateVotacaoStatus();
        }

        private void UpdateVotacaoStatus()
        {
            if (votacaoLiberada)
            {
                // Mostrar que a votação está liberada
                this.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGreen);
                this.Title = $"Urna Eletrônica - LIBERADA para: {eleitorAtual}";
                
                // Atualizar elementos visuais
                StatusBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGreen);
                StatusBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                StatusIndicator.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
                StatusText.Text = $"VOTAÇÃO LIBERADA PARA: {eleitorAtual.ToUpper()}";
                StatusText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkGreen);
            }
            else
            {
                // Mostrar que a votação está bloqueada
                this.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightCoral);
                this.Title = "Urna Eletrônica - AGUARDANDO LIBERAÇÃO DA MESA";
                
                // Atualizar elementos visuais
                StatusBorder.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightCoral);
                StatusBorder.BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                StatusIndicator.Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                StatusText.Text = "AGUARDANDO LIBERAÇÃO DA MESA";
                StatusText.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkRed);
            }
        }

        private void DigitButton_Click(object sender, RoutedEventArgs e)
        {
            if (!votacaoLiberada)
            {
                MessageBox.Show("Votação bloqueada! Aguarde a liberação da mesa receptora.", 
                              "Votação Bloqueada", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }

            if (sender is Button button && button.Tag is string digit)
            {
                AddDigit(digit);
            }
        }

        private void AddDigit(string digit)
        {
            if (!votacaoLiberada) return;

            if (currentVote.Length < 2)
            {
                currentVote += digit;
                UpdateDisplay();
                UpdateCandidateInfo();
                UpdateConfirmButton();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!votacaoLiberada) return;

            if (currentVote.Length > 0)
            {
                currentVote = currentVote.Substring(0, currentVote.Length - 1);
                UpdateDisplay();
                UpdateCandidateInfo();
                UpdateConfirmButton();
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (!votacaoLiberada) return;

            currentVote = "";
            UpdateDisplay();
            UpdateCandidateInfo();
            UpdateConfirmButton();
        }

        private void UpdateDisplay()
        {
            if (currentVote.Length >= 1)
            {
                Digit1Text.Text = currentVote[0].ToString();
            }
            else
            {
                Digit1Text.Text = "_";
            }

            if (currentVote.Length >= 2)
            {
                Digit2Text.Text = currentVote[1].ToString();
            }
            else
            {
                Digit2Text.Text = "_";
            }
        }

        private void UpdateCandidateInfo()
        {
            if (!string.IsNullOrEmpty(currentVote))
            {
                // Simular dados do candidato baseado no número
                switch (currentVote)
                {
                    case "12":
                        CandidateName.Text = "João Silva";
                        CandidateRole.Text = "Prefeito";
                        CandidateNumber.Text = "Número: 12";
                        break;
                    case "34":
                        CandidateName.Text = "Maria Santos";
                        CandidateRole.Text = "Prefeito";
                        CandidateNumber.Text = "Número: 34";
                        break;
                    case "56":
                        CandidateName.Text = "Pedro Costa";
                        CandidateRole.Text = "Prefeito";
                        CandidateNumber.Text = "Número: 56";
                        break;
                    default:
                        CandidateName.Text = "Candidato não encontrado";
                        CandidateRole.Text = "Número inválido";
                        CandidateNumber.Text = $"Número: {currentVote}";
                        break;
                }
                CandidatePanel.Visibility = Visibility.Visible;
                NoCandidatePanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                CandidatePanel.Visibility = Visibility.Collapsed;
                NoCandidatePanel.Visibility = Visibility.Visible;
            }
        }

        private void UpdateConfirmButton()
        {
            ConfirmButton.IsEnabled = !string.IsNullOrEmpty(currentVote);
        }

        private void ConfirmVote_Click(object sender, RoutedEventArgs e)
        {
            if (!votacaoLiberada)
            {
                MessageBox.Show("Votação bloqueada! Aguarde a liberação da mesa receptora.", 
                              "Votação Bloqueada", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }

            if (!string.IsNullOrEmpty(currentVote))
            {
                // Simular confirmação de voto
                MessageBox.Show($"Voto confirmado para o candidato número {currentVote}!", 
                              "Voto Confirmado", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Information);

                // Limpar após confirmação e bloquear novamente
                currentVote = "";
                UpdateDisplay();
                UpdateCandidateInfo();
                UpdateConfirmButton();
                
                // Bloquear votação após confirmação
                BloquearVotacao();
            }
        }

        private void BlankVote_Click(object sender, RoutedEventArgs e)
        {
            if (!votacaoLiberada)
            {
                MessageBox.Show("Votação bloqueada! Aguarde a liberação da mesa receptora.", 
                              "Votação Bloqueada", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }

            // Simular voto branco
            MessageBox.Show("Voto branco confirmado!", 
                          "Voto Branco", 
                          MessageBoxButton.OK, 
                          MessageBoxImage.Information);

            // Limpar após confirmação e bloquear novamente
            currentVote = "";
            UpdateDisplay();
            UpdateCandidateInfo();
            UpdateConfirmButton();
            
            // Bloquear votação após confirmação
            BloquearVotacao();
        }
    }
}