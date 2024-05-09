using R6_Downloader;
using System;
using System.Threading.Tasks;

namespace SteamKit2.Authentication
{
    /// <summary>
    /// This is a default implementation of <see cref="IAuthenticator"/> to ease of use.
    ///
    /// This implementation will prompt user to enter 2-factor authentication codes in the console.
    /// </summary>
    public class UserConsoleAuthenticator : IAuthenticator
    {
        /// <inheritdoc />
        public Task<string> GetDeviceCodeAsync( bool previousCodeWasIncorrect )
        {
            if ( previousCodeWasIncorrect )
            {
                Console.Error.WriteLine( "The previous 2-factor auth code you have provided is incorrect." );
            }

            string? code;

            do
            {
                Console.Error.Write( "STEAM GUARD! Please enter your 2-factor auth code from your authenticator app: " );
                HomePage.dispatcherQueue.TryEnqueue(async () =>
                {
                    HomePage._downloadText.Text += "\nPlease enter your 2-factor auth code from your authenticator app: ";
                    HomePage._downloadScroller.ScrollToVerticalOffset(HomePage._downloadScroller.ExtentHeight);
                });

                bool isShowingWindow = false;
                code = null;
                while (code == null)
                {
                    if (!isShowingWindow)
                    {
                        isShowingWindow = true;
                        HomePage.dispatcherQueue.TryEnqueue(async () =>
                        {
                            code = await HomePage.Show2FAInputAsync();
                        });
                    }
                }
                isShowingWindow = false;

                if( code == null )
                {
                    break;
                }
            }
            while ( string.IsNullOrEmpty( code ) );

            return Task.FromResult( code! );
        }

        /// <inheritdoc />
        public Task<string> GetEmailCodeAsync( string email, bool previousCodeWasIncorrect )
        {
            if ( previousCodeWasIncorrect )
            {
                Console.Error.WriteLine( "The previous 2-factor auth code you have provided is incorrect." );
            }

            string? code;

            do
            {
                Console.Error.Write( $"STEAM GUARD! Please enter the auth code sent to the email at {email}: " );
                HomePage.dispatcherQueue.TryEnqueue(async () =>
                {
                    HomePage._downloadText.Text += $"\nPlease enter the auth code sent to the email at {email}: ";
                    HomePage._downloadScroller.ScrollToVerticalOffset(HomePage._downloadScroller.ExtentHeight);
                });

                bool isShowingWindow = false;
                code = null;
                while (code == null)
                {
                    if (!isShowingWindow)
                    {
                        isShowingWindow = true;
                        HomePage.dispatcherQueue.TryEnqueue(async () =>
                        {
                            code = await HomePage.Show2FAInputAsync();
                        });
                    }
                }
                isShowingWindow = false;

                if ( code == null )
                {
                    break;
                }
            }
            while ( string.IsNullOrEmpty( code ) );

            return Task.FromResult( code! );
        }

        /// <inheritdoc />
        public Task<bool> AcceptDeviceConfirmationAsync()
        {
            Console.Error.WriteLine( "STEAM GUARD! Use the Steam Mobile App to confirm your sign in..." );
            HomePage.dispatcherQueue.TryEnqueue(async () =>
            {
                HomePage._downloadText.Text += $"\nUse the Steam Mobile App to confirm your sign in...";
                HomePage._downloadScroller.ScrollToVerticalOffset(HomePage._downloadScroller.ExtentHeight);
            });

            return Task.FromResult( true );
        }
    }
}
