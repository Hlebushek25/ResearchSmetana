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
                        ResearcherInterface researcherInterface = new ResearcherInterface();
                        researcherInterface.FormClosed += (s, args) => ExitThread();
                        researcherInterface.Show();
                        break;
                    case "ContinueAsAdmin":
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
                        AdminInterface adminInterface = new AdminInterface();
                        adminInterface.FormClosed += (s, args) => ExitThread();
                        adminInterface.Show();
                        break;
                    case "Exit":
                        ExitThread();
                        break;
                    default:
                        Error error = new Error("Произошла критическая ошибка\nпри запуске программы");
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
    }
}
