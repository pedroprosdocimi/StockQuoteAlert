# Welcome to Stock Quote Alert project!

In this project I developed a console project to verify if a Stock Market Value is above or below the expectations.


# Project architecture

I created five different projects inside the main project to best organize and respect the S.O.L.I.D principles.

## 1st: Batch

This part is the orchestrator of the entire project. In here you can find the **Controllers** classes, the **Program.cs** and the **appsettings.json**.
Inside **Program.cs** file there is all configuration needed to extract the information from **appsettings.json**. In there it is all the dependency injection used in this project as well.

## 2nd: Borders

In this section we store all the borders classes we need to use when running the program. **Configuration** files, **Constants**, **Dtos**, **Exceptions**, **Validators** and all **Interfaces** used.

## 3rd: Repositories

Here is where all the communication to other services are stored. In this project there is two integrations used: the **Stock Quotation** to get the current Stock price and the **Email** service to send a message everytime the price meets its expectations.

## 4th: Tests

Every bussiness rule is test here in this section. There is **Borders**, **Repositories** and **Use Cases** unit tests.

## 5th: Use Cases

In here is where the magic happen: all the logic and every bussiness rule is implemented here.


# How to start

## Appsettings

First of all, there is some previous settings we need to settle before start using the program. Inside the **Batch** project, you will see a file named Appsettings.json. In there there is three informations we need to gather before start.

The first one is the **API Key**. You need to go to the website https://hgbrasil.com/status/finance and ask for one. Don't worry, it is free! And after getting a key, you can see the **HGConsole BaseUrl** variable. In there you need to set in where url you will get the infos that you need. (Note: feel free to use any Finance API you want. Here I used this one and stored as default values.)

At last but not least, there is the **SMTPConfiguration** object where we set the communication settings to be able to send emails. I used the AWS Services https://aws.amazon.com/pt/ but you can choose anyone you like. If you don't have an account, you can create one free. After that, you will be able to authenticate the communication and send emails! (Note: if you don't know how to get the infos inside the AWS Services, in this video you can find a tutorial on how to gather them: https://www.youtube.com/watch?v=qHwFeK1gWko)

## Build your code

Now, you have to build your code and run it! Then a console will pop up waiting for an input. The input it wants is something like that: **"PETR4 15.5 24.0"**
The **PETR4** stands for the Stock Symbol.
The **15.5** stands for the minimum value of the Stock. When the Stock Market Value is below this value, the program will send an email saying that is a great time to buy some of that Stock.
And finally, the **24.0** value stands for the maximum value. It works just like the one before, but the email will be sent if the Stock Market Value is above the limit, saying that is a great time to sell.

## Any questions?

Feel free to ask me any questions!
