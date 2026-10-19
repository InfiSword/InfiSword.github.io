# Validation Guide
1. From Slime-Project, run dotnet run --project Tests/StateLifecycle/StateLifecycle.csproj. It links production lifecycle classes and uses explicit test doubles for Unity time/animation and owner objects. Expect all lifecycle checks to pass.
2. Review all direct monster behavior roots: each now derives Monster_State; concrete behavior code and constructors are unchanged.
3. With Unity 2022.3.26f1 available, compile the original project and play-test movement, attacks, dash/death and monster patrol/dash/jump. Do not substitute an editor upgrade.
4. From the website, run npm run build and npm test. On desktop/mobile, inspect chapters 2 and 4.2 and expand/collapse code using pointer and keyboard. Check source bodies match production and that fragment navigation and gallery remain functional.
