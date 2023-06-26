using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using System.Threading.Tasks;
using OpenAI;

public class WordGuessGame : MonoBehaviour
{
    [SerializeField] private Text promptText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private Text[] answerButtonsText;
    [SerializeField] private GameObject dogruPanel, yanlisPanel;
    private int randomIndex;
    private string question, answer1, answer2;
    private OpenAIApi openai;

    private  void Start()
    {
        openai = new OpenAIApi();

        answerButtons[0].onClick.AddListener(CheckBtn0);
        answerButtons[1].onClick.AddListener(CheckBtn1);

        StartGame();
    }

    public async void StartGame()
    {
        dogruPanel.SetActive(false);
        yanlisPanel.SetActive(false);
        answerButtons[0].interactable = false;
        answerButtons[1].interactable = false;
        
        randomIndex = Random.Range(0, answerButtons.Length);

        await GeneratePrompt();
        await GenerateAnswerOptions();
    }

    private async Task GeneratePrompt()
    {
        var request = new CreateCompletionRequest
        {
            Model = "text-davinci-003",
            Prompt = "Bir adet Türkçe kelime yaz."
        };

        var response = await openai.CreateCompletion(request);
        promptText.text = response.Choices[0].Text.Trim();
    }

    private async Task GenerateAnswerOptions()
    {
        await GenerateCorrectAnswer();
        await GenerateWrongAnswer();
    }

    private async Task GenerateCorrectAnswer()
    {
        var request = new CreateCompletionRequest
        {
            Model = "text-davinci-003",
            Prompt = $"{promptText.text} kelimesinin İngilizce karşılığını yaz. Tek bir kelime."
        };
        var response = await openai.CreateCompletion(request);
        //answerButtonsText[randomIndex].text = response.Choices[0].Text;
        answer1 = response.Choices[0].Text.Trim();
    }

    private async Task GenerateWrongAnswer()
    {
        var request = new CreateCompletionRequest
        {
            Model = "text-davinci-003",
            Prompt = "Rastgele bir İngilizce kelime yaz."
        };
        var response = await openai.CreateCompletion(request);
        

        int wrongAnswerIndex = (randomIndex + 1) % answerButtons.Length;
        //answerButtonsText[wrongAnswerIndex].text = response.Choices[0].Text;
        answer2 = response.Choices[0].Text.Trim();

        WriteAnswers();
    }

    void WriteAnswers()
    {
        //Cevapları butonlara yazdırdığımız bölüm
        answerButtonsText[randomIndex].text = answer1;
        
        answerButtonsText[(randomIndex + 1) % answerButtons.Length].text = answer2;
        
        answerButtons[0].interactable = true;
        answerButtons[1].interactable = true;
    }

    public void CheckBtn0()
    {
        CheckAnswer(0);
    }

    public void CheckBtn1()
    {
        CheckAnswer(1);
    }

    private void CheckAnswer(int buttonIndex)
    {
        if (randomIndex == buttonIndex)
        {
            Debug.Log("Doğru!");
            dogruPanel.SetActive(true);
        }
        else
        {
            Debug.Log("Yanlış!");
            yanlisPanel.SetActive(true);
        }
    }

    IEnumerator NextQuestion()
    {
        yield return new WaitForSeconds(3);
        StartGame();
    }
}
