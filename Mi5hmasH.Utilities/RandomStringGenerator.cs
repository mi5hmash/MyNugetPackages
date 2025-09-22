using System.Text;
using static System.Security.Cryptography.RandomNumberGenerator;

namespace Mi5hmasH.Utilities;

public static class RandomStringGenerator
{
    #region CONSTANTS
    
    private const string LatinAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Digits = "0123456789";
    private const string SpecialChars = "~!\"_#$%&'()*+,-./\\:;<=>?@[]{}^|`";
    private const string SpecialExtended = "¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƀƁƂƃƄƅƆƇƈƉƊƋƌƍƎƏƐƑƒƓƔƕƖƗƘƙƚƛƜƝƞƟƠơƢƣƤƥƦƧƨƩƪƫƬƭƮƯưƱƲƳƴƵƶƷƸƹƺƻƼƽƾƿǀǁǂǃǄǅǆǇǈǉǊǋǌǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǢǣǤǥǦǧǨǩǪǫǬǭǮǯǰǱǲǳǴǵǶǷǸǹǺǻǼǽǾǿȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚțȜȝȞȟȠȡȢȣȤȥȦȧȨȩȪȫȬȭȮȯȰȱȲȳȴȵȶȷȸȹȺȻȼȽȾȿɀɁɂɃɄɅɆɇɈɉɊɋɌɍɎɏɐɑɒɓɔɕɖɗɘəɚɛɜɝɞɟɠɡɢɣɤɥɦɧɨɩɪɫɬɭɮɯɰɱɲɳɴɵɶɷɸɹɺɻɼɽɾɿʀʁʂʃʄʅʆʇʈʉʊʋʌʍʎʏʐʑʒʓʔʕʖʗʘʙʚʛʜʝʞʟʠʡʢʣʤʥʦʧʨʩʪʫʬʭʮʯʰʱʲʳʴʵʶʷʸʹʺʻʼʽʾʿˀˁ˄˅ˆˇˈˉˊˋˌˍˎˏːˑ˒˓˔˕˖˗˘˙˚˛˝˞˟ˠˡˢˣˤ˥˦˧˨˩˪˫ˬ˭ˮ˯˰˱˲˳˴˵˶˷˸˹˺˻˼˽˾˿ͰͱͲͳʹ͵Ͷͷͺͻͼͽ;Ϳ΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϏϐϑϒϓϔϕϖϗϘϙϚϛϜϝϞϟϠϡϢϣϤϥϦϧϨϩϪϫϬϭϮϯϰϱϲϳϴϵ϶ϷϸϹϺϻϼϽϾϿЀЁЂЃЄЅІЇЈЉЊЋЌЍЎЏАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюяѐёђѓєѕіїјљњћќѝўџѠѡѢѣѤѥѦѧѨѩѪѫѬѭѮѯѰѱѲѳѴѵѶѷѸѹѺѻѼѽѾѿҀҁ҂҈҉ҊҋҌҍҎҏҐґҒғҔҕҖҗҘҙҚқҜҝҞҟҠҡҢңҤҥҦҧҨҩҪҫҬҭҮүҰұҲҳҴҵҶҷҸҹҺһҼҽҾҿӀӁӂӃӄӅӆӇӈӉӊӋӌӍӎӏӐӑӒӓӔӕӖӗӘәӚӛӜӝӞӟӠӡӢӣӤӥӦӧӨөӪӫӬӭӮӯӰӱӲӳӴӵӶӷӸӹӺӻӼӽӾӿԀԁԂԃԄԅԆԇԈԉԊԋԌԍԎԏԐԑԒԓԔԕԖԗԘԙԚԛԜԝԞԟԠԡԢԣԤԥԦԧԨԩԪԫԬԭԮԯԱԲԳԴԵԶԷԸԹԺԻԼԽԾԿՀՁՂՃՄՅՆՇՈՉՊՋՌՍՎՏՐՑՒՓՔՕՖՙ՚՛՜՝՞աբգդեզէըթժիլխծկհձղճմյնոչպջռսվտրցւփքօֆև։֊֌֍֎֏אבגדהוזחטיךכלםמןנסעףפץצקרשתװױײ׳״؀؁؂؃؄؆؇؈؉؊؋،؍؎؏ؐؑؒؓؔؕ؛؝؞؟ؠءآأؤإئابةتثجحخدذرزسشصضطظعغػؼؽؾؿـفقكلمنهوىيًٌٍَُِّْٕٖٜٟٓٔٗ٘ٙٚٛٝٞ٠١٢٣٤٥٦٧٨٩٪٫٬٭ٮٯٰٱٲٳٵٶٷٸٹٺٻټٽپٿڀځڂڃڄڅچڇڈډڊڋڌڍڎڏڐڑڒړڔڕږڗژڙښڛڜڝڞڟڠڡڢڣڤڥڦڧڨکڪګڬڭڮگڰڱڲڳڴڵڶڷڸڹںڻڼڽھڿۀہۂۃۄۅۆۇۈۉۊۋیۍێۏېۑےۓ۔ەۖۗۘۙۚۛۜ۝۞ۣ۟۠ۡۢۤۧۨ۩۪ۭ۫۬ۮۯ۰۱۲۳۴۵۶۷۸۹ۺۻۼ۽۾ۿ܀܁܂܃܄܅܆܇܈܉܊܋܌܍܎ܐܑܒܓܔܕܖܗܘܙܚܛܜܝܞܟܠܡܢܣܤܥܦܧܨܩܪܫܬܭܮܯܱܴܷܸܹܻܼܾ݂݄݆݈ܰܲܳܵܶܺܽܿ݀݁݃݅݇݉݊݋݌ݍݎݏݐݑݒݓݔݕݖݗݘݙݚݛݜݝݞݟݠݡݢݣݤݥݦݧݨݩݪݫݬݭݮݯݰݱݲݳݴݵݶݷݸݹݺݻݼݽݾݿހށނރބޅކއވމފދތލގޏސޑޒޓޔޕޖޗޘޙޚޛޜޝޞޟޠޡޢޣޤޥަާިީުޫެޭޮޯްޱ߀߁߂߃߄߅߆߇߈߉ߊߋߌߍߎߏߐߑߒߓߔߕߖߗߘߙߚߛߜߝߞߟߠߡߢߣߤߥߦߧߨߩߪ߲߫߬߭߮߯߰߱߳ߴߵ߶߷߸߹ߺ߻߽߾߿ࢠࢡࢢࢣࢤࢥࢦࢧࢨࢩࢪࢫࢬࢭࢮࢯࢰࢱࢲࢳࢴࢵࢶࢷࢸࢹࢺࢻࢼࢽઓઔક";
    
    #endregion


    private static string CreateCharsString(bool uppercaseChars, bool lowercaseChars, bool numericChars, bool specialChars, bool specialExtended)
    {
        var sb = new StringBuilder();
        if (uppercaseChars) sb.Append(LatinAlphabet);
        if (lowercaseChars) sb.Append(LatinAlphabet.ToLower());
        if (numericChars) sb.Append(Digits);
        if (specialChars) sb.Append(SpecialChars);
        if (specialExtended) sb.Append(SpecialExtended);
        return sb.ToString();
    }
    
    private static char RandomChar(string charsString)
    {
        Random rand = new();
        return charsString[rand.Next(charsString.Length)];
    }


    #region PASSWORD GENERATION

    /// <summary>
    /// Generates a password of given length.
    /// This variant allows user to specify which characters can be used.
    /// </summary>
    /// <param name="charsString">A string made of all characters that can be used.</param>
    /// <param name="length">An expected length of a result.</param>
    /// <returns></returns>
    public static string GeneratePassword(string charsString, int length = 0)
        => GetString(charsString, length);

    /// <summary>Generates a password of given length.</summary>
    /// <param name="length">An expected length of a result.</param>
    /// <param name="uppercaseChars">Determines if UPPERCASE letters of Latin Alphabet can be used.</param>
    /// <param name="lowercaseChars">Determines if LOWERCASE letters of Latin Alphabet can be used.</param>
    /// <param name="numericChars">Determines if DIGITS can be used.</param>
    /// <param name="specialChars">Determines if SPECIAL characters (like: #$%&) can be used.</param>
    /// <param name="specialExtended">Determines if EXTENDED SPECIAL characters (like: ƷƸƹƺ) can be used.</param>
    /// <returns></returns>
    public static string GeneratePassword(int length = 0, bool uppercaseChars = true, bool lowercaseChars = true, bool numericChars = true, bool specialChars = true, bool specialExtended = false)
    {
        // create charsString and return RandomString
        var charsString = CreateCharsString(uppercaseChars, lowercaseChars, numericChars, specialChars, specialExtended);
        return GetString(charsString, length);
    }

    /// <summary>Validates password against a predefined standard.</summary>
    /// <param name="input">A string to examine.</param>
    /// <param name="minLength">A minimum length the password must be.</param>
    /// <param name="maxLength">A maximum length the password must be.</param>
    /// <param name="uppercaseChars">Determines if password should contain at least one uppercase character.</param>
    /// <param name="lowercaseChars">Determines if password should contain at least one lowercase character.</param>
    /// <param name="numericChars">Determines if password should contain at least one digit.</param>
    /// <param name="specialChars">Determines if password should contain at least one special character.</param>
    /// <returns>A list of validation error messages, empty if the password is compliant.</returns>
    public static List<string> ValidatePassword(this string input, ushort minLength = 0, ushort maxLength = ushort.MaxValue,
        bool uppercaseChars = false, bool lowercaseChars = false,
        bool numericChars = false, bool specialChars = false)
    {
        var errors = new List<string>();

        if (string.IsNullOrEmpty(input))
            errors.Add("Password cannot be empty.");
        if (input.Length < minLength)
            errors.Add($"Password must be at least {minLength} characters long.");
        if (input.Length > maxLength)
            errors.Add($"Password cannot exceed {maxLength} characters.");
        if (uppercaseChars && !input.Any(char.IsUpper))
            errors.Add("Password must contain at least one uppercase letter.");
        if (lowercaseChars && !input.Any(char.IsLower))
            errors.Add("Password must contain at least one lowercase letter.");
        if (numericChars && !input.Any(char.IsDigit))
            errors.Add("Password must contain at least one digit.");
        if (specialChars && input.All(char.IsLetterOrDigit))
            errors.Add("Password must contain at least one special character.");

        return errors;
    }

    /// <summary>Calculates entropy of input string.</summary>
    /// <param name="input">A string to examine.</param>
    public static double CalculateEntropy(this string input)
    {
        // calculate poolSize
        var poolSize = 0;
        if (input.HasAtLeastOneUppercaseLetter()) poolSize += LatinAlphabet.Length;
        if (input.HasAtLeastOneLowercaseLetter()) poolSize += LatinAlphabet.Length;
        if (input.HasAtLeastOneDigit()) poolSize += Digits.Length;
        if (input.HasAtLeastOneSpecialCharacter()) poolSize += SpecialChars.Length;
        // calculate entropy and return the rounded entropy value
        var entropy = Math.Log(poolSize, 2) * input.Length;
        return Math.Round(entropy, 2);
    }

    #endregion


    #region SERIAL_KEY GENERATION

    private static string RandomSerialKey(string charsString, char separator = '-', int[]? blocks = null)
    {
        // Set default blocks if null
        blocks ??= [5, 5, 5, 5];

        // create answerParts collection then join the parts with the separator and return
        var serialParts = blocks.Select(block => GetString(charsString, block)).ToList();
        return string.Join(separator, serialParts);
    }

    private static string RandomSerialKeyFromPattern(string pattern, char patternChar, string charsString)
    {
        // split pattern to array
        var charArray = pattern.ToCharArray();
        for (var i = 0; i < pattern.Length; i++)
            if (charArray[i] == patternChar) charArray[i] = RandomChar(charsString);
        // convert charArray to string
        return new string(charArray);
    }

    /// <summary>
    /// Generates a serial-key made of blocks of strings of given length, separated by separator.
    /// This variant allows user to specify which characters can be used.
    /// </summary>
    /// <param name="charsString">A string made of all characters that can be used.</param>
    /// <param name="blocks">An array of int with each block length.</param>
    /// <param name="separator">A character to put between blocks.</param>
    /// <returns></returns>
    public static string GenerateSerialKey(string charsString, int[]? blocks = null, char separator = '-') => RandomSerialKey(charsString, separator, blocks);

    /// <summary>Generates a serial-key made of blocks of strings of given length, separated by separator.</summary>
    /// <param name="blocks">An array of int with each block length.</param>
    /// <param name="separator">A character to put between blocks.</param>
    /// <param name="uppercaseChars">Determines if UPPERCASE letters of Latin Alphabet should be used.</param>
    /// <param name="lowercaseChars">Determines if LOWERCASE letters of Latin Alphabet should be used.</param>
    /// <param name="numericChars">Determines if DIGITS should be used.</param>
    /// <returns></returns>
    public static string GenerateSerialKey(int[]? blocks = null, char separator = '-', bool uppercaseChars = true, bool lowercaseChars = false, bool numericChars = true)
    {
        // create charsString and return RandomSerialKey
        var charsString = CreateCharsString(uppercaseChars, lowercaseChars, numericChars, false, false);
        return RandomSerialKey(charsString, separator, blocks);
    }

    /// <summary>
    /// Generates a serial-key based on provided pattern.
    /// This variant allows user to specify which characters can be used.
    /// </summary>
    /// <param name="charsString">A string made of all characters that can be used.</param>
    /// <param name="pattern">The pattern of serial key to be generated (f.ex.: "RAND-xxxx-xxxx" where "x" stands for a random character).</param>
    /// <param name="patternChar">A character which stands for a random character (default is "x").</param>
    /// <returns></returns>
    public static string GenerateSerialKeyFromPattern(string charsString, string pattern = "RAND-xxxx-xxxx", char patternChar = 'x')
        => RandomSerialKeyFromPattern(pattern, patternChar, charsString);

    /// <summary>
    /// Generates a serial-key based on provided pattern.
    /// This variant allows user to specify which characters can be used.
    /// </summary>
    /// <param name="pattern">The pattern of serial key to be generated (f.ex.: "RAND-xxxx-xxxx" where "x" stands for a random character).</param>
    /// <param name="patternChar">A character which stands for a random character (default is "x").</param>
    /// <param name="uppercaseChars">Determines if UPPERCASE letters of Latin Alphabet should be used.</param>
    /// <param name="lowercaseChars">Determines if LOWERCASE letters of Latin Alphabet should be used.</param>
    /// <param name="numericChars">Determines if DIGITS should be used.</param>
    /// <returns></returns>
    public static string GenerateSerialKeyFromPattern(string pattern = "RAND-xxxx-xxxx", char patternChar = 'x', bool uppercaseChars = true, bool lowercaseChars = false, bool numericChars = true)
    {
        // create charsString and return RandomSerialKeyFromPattern
        var charsString = CreateCharsString(uppercaseChars, lowercaseChars, numericChars, false, false);
        return RandomSerialKeyFromPattern(pattern, patternChar, charsString);
    }

    #endregion
}