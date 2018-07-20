using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace HigherLowerBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        const String higher = "Higher";
        const String lower = "Lower";

        int myNumber = 10;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            myNumber = new Random().Next(1, 11);

            while (myNumber == 5)
            {
                myNumber = new Random().Next(1, 11);
            }

            PromptDialog.Choice(
                context,
                this.AfterChoiceSelected,
                new[] { higher, lower },
                String.Format("I'm thinking of a number ({0}). Is it higher or lower than 5?", myNumber),
                "I don't understand that, try guessing if my number is higher or lower then 5.",
                attempts: 5);
        }

        private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var selection = await result;
                bool isCorrect = false;

                switch (selection)
                {
                    case higher:
                        isCorrect = myNumber > 5;
                        break;

                    case lower:
                        isCorrect = myNumber < 5;
                        break;
                }

                if (isCorrect)
                {
                    await context.PostAsync("Good job! My number was " + myNumber);
                }
                else
                {
                    await context.PostAsync("Too bad, my number was " + myNumber);
                }
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Don't spam me with unknown questions.");
            }

            await this.MessageReceivedAsync(context, result);
        }
    }
}