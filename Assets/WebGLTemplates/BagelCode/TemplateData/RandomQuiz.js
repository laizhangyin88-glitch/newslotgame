function pickRandomQuizThenApply() {
  var randomQuizList = [
    {
      question: "Why are cherry symbols used frequently in slots?",
      a: "The inventor of the slot machine liked cherries.",
      b: "Retro slot levers carried a hint of cherry scent.",
      c: "It's a symbol of luck.",
      d: "Fruits used to be given out as winning prizes.",
      answer: "D"
    },
    {
      question: "Where does the term ‘jackpot’ come from?",
      a: "Jack the treasure hunter used to keep his gold in a pot inside a casino.",
      b: "In the game of ‘jacks or better’ they use the term pot for accumulated antes.",
      c: "Jackpot Inc. was the first slot machine maker to ever come up with the concept of jackpots.",
      d: "The slang for the vault doors that require spinning to open are called ‘jackpots’.",
      answer: "B"
    },
    {
      question: "Why did they start calling it ‘slots’?",
      a: "Because most slot machines have ‘coin slots’ that are used to activate the spins.",
      b: "S.L.O.T.S - is an acronym for ‘Speed’ ‘Lever’ ‘Operating’ ‘ThingS’.",
      c: "Slots is short for ‘Slotswiskov’, a name of the town where slots first originated.",
      d: "Instead of calling them ‘stalls’ they made a typo and called them ‘slots’.",
      answer: "A"
    },
    {
      question: "What is a ‘Party Bonus’ slot?",
      a: "It’s a slot game where you get bonus coins whenever someone yells ‘par-tay’.",
      b: "It's a slot game where you can play the BONUS game together with the players in the same room.",
      c: "It’s a slot which requires a party of 4 players in a same room to trigger bonus games.",
      d: "It’s a slot where you can receive multiplier bonus for the number of your party.",
      answer: "B"
    },
    {
      question: "Who is the most popular Greek God/Goddess featured on slot machines?",
      a: "Poseidon: God of sea",
      b: "Ares: God of war",
      c: "Hera: Goddess of women",
      d: "Zeus: God of thunder",
      answer: "D"
    },
    {
      question: "What is the meaning behind Chinese Dragons in casinos?",
      a: "The color of the dragon, which is green, symbolizes the color of money.",
      b: "Chinese Dragons always laid 7 orange eggs which symbolizes the lucky number, 7.",
      c: "When the Chinese Dragons roar, it makes thunder noises which symbolizes jackpots.",
      d: "Casinos adopted the Chinese dragon which symbolizes massive fortune & income.",
      answer: "D"
    },
    {
      question: "Which one of these statements is false?",
      a: "You can voluntarily ban yourself from any casinos.",
      b: "The first slot machine was placed at an auto-shop for waiting customers.",
      c: "Card counting is illegal in Black Jack.",
      d: "Penny slot machines make more money than any other games.",
      answer: "C"
    },
    {
      question: "In some countries, slot machines can be called as...",
      a: "Coin-Coin Ka-ching",
      b: "Pokies",
      c: "Buggy",
      d: "Watermelon Machines",
      answer: "B"
    }
  ];
  var randomBackgroundImageList = [
    "TemplateData/QuizBackgroundImage/V3Canvas_Quiz_V02.png",
    "TemplateData/QuizBackgroundImage/V3Canvas_Quiz_Ara_V03.png",
    "TemplateData/QuizBackgroundImage/V3Canvas_Quiz_Chameleon_V02.png",
    "TemplateData/QuizBackgroundImage/V3Canvas_Quiz_Glinda_V03.png",
    "TemplateData/QuizBackgroundImage/V3Canvas_Quiz_Papy_V03.png",
    "TemplateData/QuizBackgroundImage/V3Canvas_Quiz_Pirate_V03.png",
    "TemplateData/QuizBackgroundImage/V3Canvas_Quiz_Magician_V02.png",
    "TemplateData/QuizBackgroundImage/V3Canvas_Quiz_Matsuri_V02.png",
    "TemplateData/QuizBackgroundImage/V3Canvas_Quiz_Menou_V02.png"
  ];
  var randomQuiz = randomQuizList[Math.floor(Math.random() * randomQuizList.length)];
  var randomBackgroundImage = randomBackgroundImageList[Math.floor(Math.random() * randomBackgroundImageList.length)];
  document.getElementsByClassName('question')[0].innerHTML = randomQuiz.question;
  document.getElementsByClassName('answer')[0].innerHTML = randomQuiz.a;
  document.getElementsByClassName('answer')[1].innerHTML = randomQuiz.b;
  document.getElementsByClassName('answer')[2].innerHTML = randomQuiz.c;
  document.getElementsByClassName('answer')[3].innerHTML = randomQuiz.d;
  document.getElementsByClassName('answer-instruction')[0].innerHTML = 'The answer is ' + randomQuiz.answer + '!';
  document.getElementById('overlay').style['background-image'] = "url(" + randomBackgroundImage + ")";
}

pickRandomQuizThenApply();
