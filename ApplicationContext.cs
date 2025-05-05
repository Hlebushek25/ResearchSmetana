using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IssleduemSmetanu
{
    public class MyApplicationContext : ApplicationContext
    {
        public MyApplicationContext()
        {
            Login login = new Login();
            login.FormClosed += onLoginClosed;
            login.Show();
        }

        private async void onLoginClosed(object sender, FormClosedEventArgs e)
        {
            Login login = sender as Login;

            if (login != null)
            {
                switch (login.ActionCode)
                {
                    case "ContinueAsResearcher":
                        if (!Properties.Settings.Default.IsSoundsTurtedOff)
                        {
                            try
                            {
                                using (MemoryStream wavFile = new MemoryStream(Properties.Resources.Startup))
                                using (SoundPlayer player = new SoundPlayer(wavFile))
                                {
                                    player.Play(); // Воспроизведение звука
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка воспроизведения звука: {ex.Message}");
                            }
                        }
                        ResearcherInterface researcherInterface = new ResearcherInterface();
                        researcherInterface.FormClosed += onResearcherInterfaceClosed;
                        researcherInterface.Show();
                        break;
                    case "ContinueAsAdmin":
                        if (!Properties.Settings.Default.IsSoundsTurtedOff)
                        {
                            try
                            {
                                using (MemoryStream wavFile = new MemoryStream(Properties.Resources.Startup))
                                using (SoundPlayer player = new SoundPlayer(wavFile))
                                {
                                    player.Play(); // Воспроизведение звука
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка воспроизведения звука: {ex.Message}");
                            }
                        }
                        AdminInterface adminInterface = new AdminInterface();
                        adminInterface.FormClosed += onAdminInterfaceClosed;
                        adminInterface.Show();
                        break;
                    case "Exit":
                        ExitThread();
                        break;
                    default:
                        Dialog error = new Dialog("Произошла критическая ошибка\nпри запуске программы", DialogType.Error);
                        error.Show();

                        await Task.Delay(3000);

                        BSOD bsod = new BSOD();
                        bsod.FormClosed += (s, args) => ExitThread();
                        bsod.Show();

                        await Task.Delay(1000);
                        error.Close();
                        break;
                }
            }
        }

        private async void onAdminInterfaceClosed(object sender, FormClosedEventArgs e)
        {
            AdminInterface adminInterface = sender as AdminInterface;

            if (adminInterface != null)
            {
                switch (adminInterface.ActionCode)
                {
                    case "Logout":
                        if (!Properties.Settings.Default.IsSoundsTurtedOff)
                        {
                            try
                            {
                                using (MemoryStream wavFile = new MemoryStream(Properties.Resources.Logout))
                                using (SoundPlayer player = new SoundPlayer(wavFile))
                                {
                                    player.Play(); // Воспроизведение звука
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка воспроизведения звука: {ex.Message}");
                            }
                        }
                        Login login = new Login();
                        login.FormClosed += onLoginClosed;
                        login.Show();
                        break;
                    default:
                        ExitThread();
                        break;
                }
            }
        }

        private async void onResearcherInterfaceClosed(object sender, FormClosedEventArgs e)
        {
            ResearcherInterface researcherInterface = sender as ResearcherInterface;

            if (researcherInterface != null)
            {
                switch (researcherInterface.ActionCode)
                {
                    case "Logout":
                        if (!Properties.Settings.Default.IsSoundsTurtedOff)
                        {
                            try
                            {
                                using (MemoryStream wavFile = new MemoryStream(Properties.Resources.Logout))
                                using (SoundPlayer player = new SoundPlayer(wavFile))
                                {
                                    player.Play(); // Воспроизведение звука
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Ошибка воспроизведения звука: {ex.Message}");
                            }
                        }
                        Login login = new Login();
                        login.FormClosed += onLoginClosed;
                        login.Show();
                        break;
                    default:
                        ExitThread();
                        break;
                }
            }
        }
    }
}
