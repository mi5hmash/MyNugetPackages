using System.Text;
using static System.Security.Cryptography.RandomNumberGenerator;

namespace Mi5hmasH.Utilities.Helpers;

public static class RandomPasswordGenerator
{
    public const string LatinAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string Digits = "0123456789";
    public const string SpecialChars = @"~!""_#$%&'()*+,-./\:;<=>?@[]{}^|`";
    public const string SpecialExtended = "¡¢£¤¥¦§¨©ª«¬­®¯°±²³´µ¶·¸¹º»¼½¾¿ÀÁÂÃÄÅÆÇÈÉÊËÌÍÎÏÐÑÒÓÔÕÖ×ØÙÚÛÜÝÞßàáâãäåæçèéêëìíîïðñòóôõö÷øùúûüýþÿĀāĂăĄąĆćĈĉĊċČčĎďĐđĒēĔĕĖėĘęĚěĜĝĞğĠġĢģĤĥĦħĨĩĪīĬĭĮįİıĲĳĴĵĶķĸĹĺĻļĽľĿŀŁłŃńŅņŇňŉŊŋŌōŎŏŐőŒœŔŕŖŗŘřŚśŜŝŞşŠšŢţŤťŦŧŨũŪūŬŭŮůŰűŲųŴŵŶŷŸŹźŻżŽžſƀƁƂƃƄƅƆƇƈƉƊƋƌƍƎƏƐƑƒƓƔƕƖƗƘƙƚƛƜƝƞƟƠơƢƣƤƥƦƧƨƩƪƫƬƭƮƯưƱƲƳƴƵƶƷƸƹƺƻƼƽƾƿǀǁǂǃǄǅǆǇǈǉǊǋǌǍǎǏǐǑǒǓǔǕǖǗǘǙǚǛǜǝǞǟǠǡǢǣǤǥǦǧǨǩǪǫǬǭǮǯǰǱǲǳǴǵǶǷǸǹǺǻǼǽǾǿȀȁȂȃȄȅȆȇȈȉȊȋȌȍȎȏȐȑȒȓȔȕȖȗȘșȚțȜȝȞȟȠȡȢȣȤȥȦȧȨȩȪȫȬȭȮȯȰȱȲȳȴȵȶȷȸȹȺȻȼȽȾȿɀɁɂɃɄɅɆɇɈɉɊɋɌɍɎɏɐɑɒɓɔɕɖɗɘəɚɛɜɝɞɟɠɡɢɣɤɥɦɧɨɩɪɫɬɭɮɯɰɱɲɳɴɵɶɷɸɹɺɻɼɽɾɿʀʁʂʃʄʅʆʇʈʉʊʋʌʍʎʏʐʑʒʓʔʕʖʗʘʙʚʛʜʝʞʟʠʡʢʣʤʥʦʧʨʩʪʫʬʭʮʯʰʱʲʳʴʵʶʷʸʹʺʻʼʽʾʿˀˁ˄˅ˆˇˈˉˊˋˌˍˎˏːˑ˒˓˔˕˖˗˘˙˚˛˝˞˟ˠˡˢˣˤ˥˦˧˨˩˪˫ˬ˭ˮ˯˰˱˲˳˴˵˶˷˸˹˺˻˼˽˾˿ͰͱͲͳʹ͵Ͷͷͺͻͼͽ;Ϳ΄΅Ά·ΈΉΊΌΎΏΐΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΪΫάέήίΰαβγδεζηθικλμνξοπρςστυφχψωϊϋόύώϏϐϑϒϓϔϕϖϗϘϙϚϛϜϝϞϟϠϡϢϣϤϥϦϧϨϩϪϫϬϭϮϯϰϱϲϳϴϵ϶ϷϸϹϺϻϼϽϾϿЀЁЂЃЄЅІЇЈЉЊЋЌЍЎЏАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюяѐёђѓєѕіїјљњћќѝўџѠѡѢѣѤѥѦѧѨѩѪѫѬѭѮѯѰѱѲѳѴѵѶѷѸѹѺѻѼѽѾѿҀҁ҂҈҉ҊҋҌҍҎҏҐґҒғҔҕҖҗҘҙҚқҜҝҞҟҠҡҢңҤҥҦҧҨҩҪҫҬҭҮүҰұҲҳҴҵҶҷҸҹҺһҼҽҾҿӀӁӂӃӄӅӆӇӈӉӊӋӌӍӎӏӐӑӒӓӔӕӖӗӘәӚӛӜӝӞӟӠӡӢӣӤӥӦӧӨөӪӫӬӭӮӯӰӱӲӳӴӵӶӷӸӹӺӻӼӽӾӿԀԁԂԃԄԅԆԇԈԉԊԋԌԍԎԏԐԑԒԓԔԕԖԗԘԙԚԛԜԝԞԟԠԡԢԣԤԥԦԧԨԩԪԫԬԭԮԯԱԲԳԴԵԶԷԸԹԺԻԼԽԾԿՀՁՂՃՄՅՆՇՈՉՊՋՌՍՎՏՐՑՒՓՔՕՖՙ՚՛՜՝՞աբգդեզէըթժիլխծկհձղճմյնոչպջռսվտրցւփքօֆև։֊֌֍֎֏אבגדהוזחטיךכלםמןנסעףפץצקרשתװױײ׳״؀؁؂؃؄؆؇؈؉؊؋،؍؎؏ؐؑؒؓؔؕ؛؝؞؟ؠءآأؤإئابةتثجحخدذرزسشصضطظعغػؼؽؾؿـفقكلمنهوىيًٌٍَُِّْٕٖٜٟٓٔٗ٘ٙٚٛٝٞ٠١٢٣٤٥٦٧٨٩٪٫٬٭ٮٯٰٱٲٳٵٶٷٸٹٺٻټٽپٿڀځڂڃڄڅچڇڈډڊڋڌڍڎڏڐڑڒړڔڕږڗژڙښڛڜڝڞڟڠڡڢڣڤڥڦڧڨکڪګڬڭڮگڰڱڲڳڴڵڶڷڸڹںڻڼڽھڿۀہۂۃۄۅۆۇۈۉۊۋیۍێۏېۑےۓ۔ەۖۗۘۙۚۛۜ۝۞ۣ۟۠ۡۢۤۧۨ۩۪ۭ۫۬ۮۯ۰۱۲۳۴۵۶۷۸۹ۺۻۼ۽۾ۿ܀܁܂܃܄܅܆܇܈܉܊܋܌܍܎ܐܑܒܓܔܕܖܗܘܙܚܛܜܝܞܟܠܡܢܣܤܥܦܧܨܩܪܫܬܭܮܯܱܴܷܸܹܻܼܾ݂݄݆݈ܰܲܳܵܶܺܽܿ݀݁݃݅݇݉݊݋݌ݍݎݏݐݑݒݓݔݕݖݗݘݙݚݛݜݝݞݟݠݡݢݣݤݥݦݧݨݩݪݫݬݭݮݯݰݱݲݳݴݵݶݷݸݹݺݻݼݽݾݿހށނރބޅކއވމފދތލގޏސޑޒޓޔޕޖޗޘޙޚޛޜޝޞޟޠޡޢޣޤޥަާިީުޫެޭޮޯްޱ߀߁߂߃߄߅߆߇߈߉ߊߋߌߍߎߏߐߑߒߓߔߕߖߗߘߙߚߛߜߝߞߟߠߡߢߣߤߥߦߧߨߩߪ߲߫߬߭߮߯߰߱߳ߴߵ߶߷߸߹ߺ߻߽߾߿ࢠࢡࢢࢣࢤࢥࢦࢧࢨࢩࢪࢫࢬࢭࢮࢯࢰࢱࢲࢳࢴࢵࢶࢷࢸࢹࢺࢻࢼࢽઓઔક";
    
    public static string CreateCharsSourceString(bool uppercaseChars, bool lowercaseChars, bool numericChars, bool specialChars, bool specialExtended)
    {
        var sb = new StringBuilder();
        if (uppercaseChars) sb.Append(LatinAlphabet);
        if (lowercaseChars) sb.Append(LatinAlphabet.ToLower());
        if (numericChars) sb.Append(Digits);
        if (specialChars) sb.Append(SpecialChars);
        if (specialExtended) sb.Append(SpecialExtended);
        return sb.ToString();
    }

    private static readonly Random Rand = new();

    /// <param name="charsSourceString">A string made of all characters that can be used.</param>
    extension(string charsSourceString)
    {
        /// <summary>
        /// Returns a random character from a string.
        /// </summary>
        /// <returns>A random character from a string.</returns>
        public char GetRandomChar() 
            => charsSourceString[Rand.Next(charsSourceString.Length)];

        /// <summary>
        /// Generates an array of random characters of the specified length.
        /// </summary>
        /// <param name="numberOfChars">The number of characters to generate.</param>
        /// <returns>An array of randomly selected characters.</returns>
        public char[] GetRandomChars(int numberOfChars)
        {
            var result = new char[numberOfChars];
            for (var i = 0; i < numberOfChars; i++) 
                result[i] = charsSourceString[Rand.Next(charsSourceString.Length)];
        
            return result;
        }
    }

    /// <summary>
    /// Generates a password of given length.
    /// This variant allows user to specify which characters can be used.
    /// </summary>
    /// <param name="charsSourceString">A string made of all characters that can be used.</param>
    /// <param name="length">An expected length of a result.</param>
    /// <returns></returns>
    public static string GeneratePassword(string charsSourceString, int length = 0)
        => GetString(charsSourceString, length);

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
        var charsSourceString = CreateCharsSourceString(uppercaseChars, lowercaseChars, numericChars, specialChars, specialExtended);
        return GetString(charsSourceString, length);
    }
}