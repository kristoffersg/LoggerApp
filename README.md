# Et par ord om, hvad der er fixet og ændret
## Bugs
1. AsyncLogger.cs - linje 51
   Der bliver itereret direkte på _line objected, som undervejs i loopet ændrer sig. Dette virker ikke og der er derfor .ToArray() på _line for at kopiere til et array, som der itereret på i stedet for.
2. AsyncLogger.cs - linje 93
   Checket for om der er skiftet dato virker ikke, og er blevet ændret.

## Generelle ændringer
* private fields i AsyncLogger.cs er alle flytte samlet til toppen af klassen.
* Constructoren i AsyncLogger.cs er lavet mere clean og indeholder nu kun DI.
* DateTime.Now er abstraheret væk til SystemClock.cs og ISystemClock, for at kunne mock den og fake en datoændring.
* CreateNewFile() i AsyncLogger.cs er lavet for at gøre koden mere læsbar, og for ikke at have duplikeret kode flere steder.
* CheckDate() i AsyncLogger.cs er også lavet for at gøre kode mere lærbar.
* StartLogger() i Program.cs er tilføjet for at kunne starte tasks herfra. Dette er gjort for at kunne gribe exceptions fra main thread. Hertil er ExceptionHandler() lavet for at gribe exceptions fra Tasks.
* Threads er byttet ud med Tasks for at kunne gribe exceptions.
* Generelt er der lavet lidt navneændringer rundt omkring:
  - LogInterface -> IAsyncLogger
  - AsyncLogInterface -> AsyncLogger
  - "_" er fjernet fra diverse metodenavne
  - this. er fjernet fra fields da der også bruges "_"
