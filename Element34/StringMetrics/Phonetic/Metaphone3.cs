using System.Text;

namespace Element34.StringMetrics
{
    public class Metaphone3 : IStringMetaphoneEncoder, IStringComparison
    {

        #region Fields
        private int m_length;
        private int m_metaphLength;
        private bool m_encodeVowels;
        private bool m_encodeExact;
        private string m_inWord;
        private StringBuilder m_primary;
        private StringBuilder m_secondary;
        private bool m_hasAlternate;            //Flag indicating if an alternate metaphone key was computed for the word
        private int m_current;
        private int m_last;
        private bool flag_AL_inversion;
        private const int MAX_KEY_ALLOCATION = 32;
        private const int DEFAULT_MAX_KEY_LENGTH = 8;
        #endregion

        /// /////////////////////////////////////////////////////////////////////////////
        //  Metaphone3 class definition
        /// /////////////////////////////////////////////////////////////////////////////
        public Metaphone3()
        {
            m_primary = new StringBuilder();
            m_secondary = new StringBuilder();
            m_metaphLength = DEFAULT_MAX_KEY_LENGTH;
            m_encodeVowels = false;
            m_encodeExact = false;
        }

        public Metaphone3(string sInput) : this()
        {
            Encode(sInput);
        }

        public string PrimaryKey
        {
            get
            {
                return m_primary.ToString();
            }
        }

        public string AlternateKey
        {
            get
            {
                return m_hasAlternate ? m_secondary.ToString() : string.Empty;
            }
        }

        public string Word
        {
            get
            {
                return m_inWord;
            }
        }

        private bool SetKeyLength(int inKeyLength)
        {
            if ((inKeyLength < 1))
            {
                //  can't have that -
                //  no room for terminating null
                inKeyLength = 1;
            }

            if ((inKeyLength > MAX_KEY_ALLOCATION))
            {
                m_metaphLength = MAX_KEY_ALLOCATION;
                return false;
            }

            m_metaphLength = inKeyLength;
            return true;
        }

        private void MetaphAdd(string sInput)
        {
            if (!(sInput.Equals("A")
                        && ((m_primary.Length > 0)
                        && (m_primary.ToString()[(m_primary.Length - 1)] == 'A'))))
            {
                m_primary.Append(sInput);
            }

            if (!(sInput.Equals("A")
                        && ((m_secondary.Length > 0)
                        && (m_secondary.ToString()[(m_secondary.Length - 1)] == 'A'))))
            {
                m_hasAlternate = true;
                m_secondary.Append(sInput);
            }

        }

        private void MetaphAdd(string main, string alt)
        {
            if (!(main.Equals("A")
                        && ((m_primary.Length > 0)
                        && (m_primary.ToString()[(m_primary.Length - 1)] == 'A'))))
            {
                m_primary.Append(main);
            }

            if (!(alt.Equals("A")
                        && ((m_secondary.Length > 0)
                        && (m_secondary.ToString()[(m_secondary.Length - 1)] == 'A'))))
            {
                if (!string.IsNullOrEmpty(alt))
                {
                    m_hasAlternate = true;
                    m_secondary.Append(alt);
                }
            }
        }

        private void MetaphAddExactApprox(string mainExact, string altExact, string main, string alt)
        {
            if (m_encodeExact)
            {
                MetaphAdd(mainExact, altExact);
            }
            else
            {
                MetaphAdd(main, alt);
            }
        }

        private void MetaphAddExactApprox(string mainExact, string main)
        {
            if (m_encodeExact)
            {
                MetaphAdd(mainExact);
            }
            else
            {
                MetaphAdd(main);
            }
        }

        private int GetKeyLength()
        {
            return m_metaphLength;
        }

        private int GetMaximumKeyLength()
        {
            return MAX_KEY_ALLOCATION;
        }

        private void SetEncodeVowels(bool inEncodeVowels)
        {
            m_encodeVowels = inEncodeVowels;
        }

        private bool GetEncodeVowels()
        {
            return m_encodeVowels;
        }

        private void SetEncodeExact(bool inEncodeExact)
        {
            m_encodeExact = inEncodeExact;
        }

        private bool GetEncodeExact()
        {
            return m_encodeExact;
        }

        string GetMetaph()
        {
            return m_primary.ToString();
        }

        string GetAlternateMetaph()
        {
            return m_secondary.ToString();
        }

        private bool Front_Vowel(int at)
        {
            if (((CharAt(at) == 'E')
                        || ((CharAt(at) == 'I')
                        || (CharAt(at) == 'Y'))))
            {
                return true;
            }

            return false;
        }

        private bool SlavoGermanic()
        {
            if ((StringAt(0, 3, "SCH", "")
                        || (StringAt(0, 2, "SW", "")
                        || ((CharAt(0) == 'J')
                        || (CharAt(0) == 'W')))))
            {
                return true;
            }

            return false;
        }

        private bool IsVowel(char inChar)
        {
            if ((inChar == 'A') || (inChar == 'E') || (inChar == 'I') || (inChar == 'O') || (inChar == 'U')
                    || (inChar == 'Y') || (inChar == 'À') || (inChar == 'Á') || (inChar == 'Â') || (inChar == 'Ã')
                    || (inChar == 'Ä') || (inChar == 'Å') || (inChar == 'Æ') || (inChar == 'È') || (inChar == 'É')
                    || (inChar == 'Ê') || (inChar == 'Ë') || (inChar == 'Ì') || (inChar == 'Í') || (inChar == 'Î')
                    || (inChar == 'Ï') || (inChar == 'Ò') || (inChar == 'Ó') || (inChar == 'Ô') || (inChar == 'Õ')
                    || (inChar == 'Ö') || (inChar == '') || (inChar == 'Ø') || (inChar == 'Ù') || (inChar == 'Ú')
                    || (inChar == 'Û') || (inChar == 'Ü') || (inChar == 'Ý') || (inChar == ''))
            {
                return true;
            }

            return false;
        }

        private bool IsVowel(int at)
        {
            if ((at < 0) || (at >= m_length))
            {
                return false;
            }

            char it = CharAt(at);

            if (IsVowel(it))
            {
                return true;
            }

            return false;
        }

        private int SkipVowels(int at)
        {
            if (at < 0)
            {
                return 0;
            }

            if (at >= m_length)
            {
                return m_length;
            }

            char it = CharAt(at);

            while (IsVowel(it) || (it == 'W'))
            {
                if (StringAt(at, 4, "WICZ", "WITZ", "WIAK", "")
                        || StringAt((at - 1), 5, "EWSKI", "EWSKY", "OWSKI", "OWSKY", "")
                        || (StringAt(at, 5, "WICKI", "WACKI", "") && ((at + 4) == m_last)))
                {
                    break;
                }

                at++;
                if (((CharAt(at - 1) == 'W') && (CharAt(at) == 'H')) && !(StringAt(at, 3, "HOP", "")
                        || StringAt(at, 4, "HIDE", "HARD", "HEAD", "HAWK", "HERD", "HOOK", "HAND", "HOLE", "")
                        || StringAt(at, 5, "HEART", "HOUSE", "HOUND", "") || StringAt(at, 6, "HAMMER", "")))
                {
                    at++;
                }

                if (at > (m_length - 1))
                {
                    break;
                }
                it = CharAt(at);
            }

            return at;
        }

        private void AdvanceCounter(int ifNotEncodeVowels, int ifEncodeVowels)
        {
            if (!m_encodeVowels)
            {
                m_current += ifNotEncodeVowels;
            }
            else
            {
                m_current += ifEncodeVowels;
            }
        }

        private char CharAt(int at)
        {
            // check substring bounds
            if ((at < 0) || (at > (m_length - 1)))
            {
                return '\0';
            }

            return m_inWord[at];
        }

        private bool RootOrInflections(string sInputWord, string root)
        {
            int len = root.Length;
            string test;
            test = (root + "S");
            if ((sInputWord.Equals(root) || sInputWord.Equals(test)))
            {
                return true;
            }

            if ((root[(len - 1)] != 'E'))
            {
                test = (root + "ES");
            }

            if (sInputWord.Equals(test))
            {
                return true;
            }

            if ((root[(len - 1)] != 'E'))
            {
                test = (root + "ED");
            }
            else
            {
                test = (root + "D");
            }

            if (sInputWord.Equals(test))
            {
                return true;
            }

            if ((root[(len - 1)] == 'E'))
            {
                root = root.Substring(0, (len - 1));
            }

            test = (root + "ING");
            if (sInputWord.Equals(test))
            {
                return true;
            }

            test = (root + "INGLY");
            if (sInputWord.Equals(test))
            {
                return true;
            }

            test = (root + "Y");
            if (sInputWord.Equals(test))
            {
                return true;
            }

            return false;
        }

        private bool StringAt(int start, int length, params string[] comparestrings)
        {
            //  check substring bounds
            if ((start < 0) || ((start > (m_length - 1))) || ((start + (length - 1)) > (m_length - 1)) || (start + start + length) > (m_inWord.Length - 1))
            {
                return false;
            }

            string target = m_inWord.Substring(start, (start + length));
            foreach (string strFragment in comparestrings)
            {
                if (target.Equals(strFragment))
                {
                    return true;
                }

            }

            return false;
        }

        public void Encode(string sInput)
        {
            m_inWord = sInput.ToUpper();
            m_length = m_inWord.Length;

            flag_AL_inversion = false;

            m_current = 0;

            m_primary.Length = 0;
            m_secondary.Length = 0;

            m_hasAlternate = false;

            if (m_length < 1)
            {
                return;
            }

            // zero based index
            m_last = m_length - 1;

            /////////// main loop//////////////////////////
            while (m_primary.Length <= m_metaphLength || m_secondary.Length <= m_metaphLength)
            {
                if (m_current >= m_length)
                {
                    break;
                }
                char c = CharAt(m_current);

                switch (c)
                {
                    case 'B':

                        Encode_B();
                        break;

                    case 'ß':
                    case 'Ç':

                        MetaphAdd("S");
                        m_current++;
                        break;

                    case 'C':

                        Encode_C();
                        break;

                    case 'D':

                        Encode_D();
                        break;

                    case 'F':

                        Encode_F();
                        break;

                    case 'G':

                        Encode_G();
                        break;

                    case 'H':

                        Encode_H();
                        break;

                    case 'J':

                        Encode_J();
                        break;

                    case 'K':

                        Encode_K();
                        break;

                    case 'L':

                        Encode_L();
                        break;

                    case 'M':

                        Encode_M();
                        break;

                    case 'N':

                        Encode_N();
                        break;

                    case 'Ñ':

                        MetaphAdd("N");
                        m_current++;
                        break;

                    case 'P':

                        Encode_P();
                        break;

                    case 'Q':

                        Encode_Q();
                        break;

                    case 'R':

                        Encode_R();
                        break;

                    case 'S':

                        Encode_S();
                        break;

                    case 'T':

                        Encode_T();
                        break;

                    case 'Ð': // eth
                    case 'Þ': // thorn

                        MetaphAdd("0");
                        m_current++;
                        break;

                    case 'V':

                        Encode_V();
                        break;

                    case 'W':

                        Encode_W();
                        break;

                    case 'X':

                        Encode_X();
                        break;

                    case '':

                        MetaphAdd("X");
                        m_current++;
                        break;

                    case '':

                        MetaphAdd("S");
                        m_current++;
                        break;

                    case 'Z':

                        Encode_Z();
                        break;

                    default:

                        if (IsVowel(CharAt(m_current)))
                        {
                            Encode_Vowels();
                        }
                        else
                        {
                            m_current++;
                        }
                        break;
                }
            }

            // only give back m_metaphLength number of chars in m_metaph
            if (m_primary.Length > m_metaphLength)
            {
                m_primary.Length = m_metaphLength;
            }

            if (m_secondary.Length > m_metaphLength)
            {
                m_secondary.Length = m_metaphLength;
            }

            // it is possible for the two metaphs to be the same
            // after truncation. lose the second one if so
            if ((m_primary.ToString()).Equals(m_secondary.ToString()))
            {
                m_hasAlternate = false;
                m_secondary.Length = 0;
            }
        }

        private void Encode_Vowels()
        {
            if ((m_current == 0))
            {
                //  all init vowels map to 'A' 
                //  as of Double Metaphone
                MetaphAdd("A");
            }
            else if (m_encodeVowels)
            {
                if ((CharAt(m_current) != 'E'))
                {
                    if (Skip_Silent_UE())
                    {
                        return;
                    }

                    if (O_Silent())
                    {
                        m_current++;
                        return;
                    }

                    //  encode all vowels and
                    //  diphthongs to the same value
                    MetaphAdd("A");
                }
                else
                {
                    Encode_E_Pronounced();
                }

            }

            if (!(!IsVowel((m_current - 2))
                        && StringAt((m_current - 1), 4, "LEWA", "LEWO", "LEWI", "")))
            {
                m_current = SkipVowels(m_current);
            }
            else
            {
                m_current++;
            }
        }

        private void Encode_E_Pronounced()
        {
            //  special cases with two pronunciations
            //  'agape' 'lame' 'resume'
            if (((StringAt(0, 4, "LAME", "SAKE", "PATE", "")
                        && (m_length == 4))
                        || ((StringAt(0, 5, "AGAPE", "")
                        && (m_length == 5))
                        || ((m_current == 5)
                        && StringAt(0, 6, "RESUME", "")))))
            {
                MetaphAdd("", "A");
                return;
            }

            //  special case "inge" => 'INGA', 'INJ'
            if ((StringAt(0, 4, "INGE", "")
                        && (m_length == 4)))
            {
                MetaphAdd("A", "");
                return;
            }

            //  special cases with two pronunciations
            //  special handling due to the difference in
            //  the pronunciation of the '-D'
            if (((m_current == 5)
                        && StringAt(0, 7, "BLESSED", "LEARNED", "")))
            {
                MetaphAddExactApprox("D", "AD", "T", "AT");
                m_current += 2;
                return;
            }

            //  encode all vowels and diphthongs to the same value
            if (((!E_Silent()
                        && (!flag_AL_inversion
                        && !Silent_Internal_E()))
                        || E_Pronounced_Exceptions()))
            {
                MetaphAdd("A");
            }

            //  now that we've visited the vowel in question
            flag_AL_inversion = false;
        }

        private bool O_Silent()
        {
            //  if "iron" at beginning or end of word and not "irony"
            if (((CharAt(m_current) == 'O')
                        && StringAt((m_current - 2), 4, "IRON", "")))
            {
                if (((StringAt(0, 4, "IRON", "")
                            || (StringAt((m_current - 2), 4, "IRON", "")
                            && (m_last
                            == (m_current + 1))))
                            && !StringAt((m_current - 2), 6, "IRONIC", "")))
                {
                    return true;
                }
            }

            return false;
        }

        private bool E_Silent()
        {
            if (E_Pronounced_At_End())
            {
                return false;
            }

            //  'e' silent when last letter, altho
            if (((m_current == m_last)
                        || ((StringAt(m_last, 1, "S", "D", "")
                        && ((m_current > 1)
                        && (((m_current + 1)
                        == m_last)
                        && !(StringAt((m_current - 1), 3, "TED", "SES", "CES", "")
                        || (StringAt(0, 9, "ANTIPODES", "ANOPHELES", "")
                        || (StringAt(0, 8, "MOHAMMED", "MUHAMMED", "MOUHAMED", "")
                        || (StringAt(0, 7, "MOHAMED", "")
                        || (StringAt(0, 6, "NORRED", "MEDVED", "MERCED", "ALLRED", "KHALED", "RASHED", "MASJED", "")
                        || (StringAt(0, 5, "JARED", "AHMED", "HAMED", "JAVED", "") || StringAt(0, 4, "ABED", "IMED", ""))))))))))
                        || ((StringAt((m_current + 1), 4, "NESS", "LESS", "")
                        && ((m_current + 4)
                        == m_last))
                        || (StringAt((m_current + 1), 2, "LY", "")
                        && (((m_current + 2)
                        == m_last)
                        && !StringAt(0, 6, "CICELY", "")))))))
            {
                return true;
            }

            return false;
        }

        private bool E_Pronounced_At_End()
        {
            if ((m_current == m_last) && (StringAt((m_current - 6), 7, "STROPHE", "")
                    // if a vowel is before the 'E', vowel eater will have eaten it.
                    // otherwise, consonant + 'E' will need 'E' pronounced
                    || (m_length == 2) || ((m_length == 3) && !IsVowel(0))
                    // these German name endings can be relied on to have the 'e' pronounced
                    || (StringAt((m_last - 2), 3, "BKE", "DKE", "FKE", "KKE", "LKE", "NKE", "MKE", "PKE", "TKE", "VKE",
                            "ZKE", "") && !StringAt(0, 5, "FINKE", "FUNKE", "") && !StringAt(0, 6, "FRANKE", ""))
                    || StringAt((m_last - 4), 5, "SCHKE", "")
                    || (StringAt(0, 4, "ACME", "NIKE", "CAFE", "RENE", "LUPE", "JOSE", "ESME", "") && (m_length == 4))
                    || (StringAt(0, 5, "LETHE", "CADRE", "TILDE", "SIGNE", "POSSE", "LATTE", "ANIME", "DOLCE", "CROCE",
                            "ADOBE", "OUTRE", "JESSE", "JAIME", "JAFFE", "BENGE", "RUNGE", "CHILE", "DESME", "CONDE",
                            "URIBE", "LIBRE", "ANDRE", "") && (m_length == 5))
                    || (StringAt(0, 6, "HECATE", "PSYCHE", "DAPHNE", "PENSKE", "CLICHE", "RECIPE", "TAMALE", "SESAME",
                            "SIMILE", "FINALE", "KARATE", "RENATE", "SHANTE", "OBERLE", "COYOTE", "KRESGE", "STONGE",
                            "STANGE", "SWAYZE", "FUENTE", "SALOME", "URRIBE", "") && (m_length == 6))
                    || (StringAt(0, 7, "ECHIDNE", "ARIADNE", "MEINEKE", "PORSCHE", "ANEMONE", "EPITOME", "SYNCOPE",
                            "SOUFFLE", "ATTACHE", "MACHETE", "KARAOKE", "BUKKAKE", "VICENTE", "ELLERBE", "VERSACE", "")
                            && (m_length == 7))
                    || (StringAt(0, 8, "PENELOPE", "CALLIOPE", "CHIPOTLE", "ANTIGONE", "KAMIKAZE", "EURIDICE", "YOSEMITE",
                            "FERRANTE", "") && (m_length == 8))
                    || (StringAt(0, 9, "HYPERBOLE", "GUACAMOLE", "XANTHIPPE", "") && (m_length == 9))
                    || (StringAt(0, 10, "SYNECDOCHE", "") && (m_length == 10))))
            {
                return true;
            }

            return false;
        }

        private bool Silent_Internal_E()
        {
            //  'olesen' but not 'olen'    RAKE BLAKE 
            if (((StringAt(0, 3, "OLE", "")
                        && (E_Silent_Suffix(3)
                        && !E_Pronouncing_Suffix(3)))
                        || ((StringAt(0, 4, "BARE", "FIRE", "FORE", "GATE", "HAGE", "HAVE", "HAZE", "HOLE", "CAPE", "HUSE", "LACE", "LINE", "LIVE", "LOVE", "MORE", "MOSE", "MORE", "NICE", "RAKE", "ROBE", "ROSE", "SISE", "SIZE", "WARE", "WAKE", "WISE", "WINE", "")
                        && (E_Silent_Suffix(4)
                        && !E_Pronouncing_Suffix(4)))
                        || ((StringAt(0, 5, "BLAKE", "BRAKE", "BRINE", "CARLE", "CLEVE", "DUNNE", "HEDGE", "HOUSE", "JEFFE", "LUNCE", "STOKE", "STONE", "THORE", "WEDGE", "WHITE", "")
                        && (E_Silent_Suffix(5)
                        && !E_Pronouncing_Suffix(5)))
                        || ((StringAt(0, 6, "BRIDGE", "CHEESE", "")
                        && (E_Silent_Suffix(6)
                        && !E_Pronouncing_Suffix(6)))
                        || StringAt((m_current - 5), 7, "CHARLES", ""))))))
            {
                return true;
            }

            return false;
        }

        private bool E_Silent_Suffix(int at)
        {
            if ((m_current == (at - 1)) && (m_length > (at + 1))
                    && (IsVowel((at + 1)) || (StringAt(at, 2, "ST", "SL", "") && (m_length > (at + 2)))))
            {
                return true;
            }

            return false;
        }

        private bool E_Pronouncing_Suffix(int at)
        {
            // e.g. 'bridgewood' - the other vowels will get eaten
            // up so we need to put one in here
            if ((m_length == (at + 4)) && StringAt(at, 4, "WOOD", ""))
            {
                return true;
            }

            // same as above
            if ((m_length == (at + 5)) && StringAt(at, 5, "WATER", "WORTH", ""))
            {
                return true;
            }

            // e.g. 'Bridgette'
            if ((m_length == (at + 3)) && StringAt(at, 3, "TTE", "LIA", "NOW", "ROS", "RAS", ""))
            {
                return true;
            }

            // e.g. 'Olena'
            if ((m_length == (at + 2))
                    && StringAt(at, 2, "TA", "TT", "NA", "NO", "NE", "RS", "RE", "LA", "AU", "RO", "RA", ""))
            {
                return true;
            }

            // e.g. 'Bridget'
            if ((m_length == (at + 1)) && StringAt(at, 1, "T", "R", ""))
            {
                return true;
            }

            return false;
        }

        private bool E_Pronounced_Exceptions()
        {
            //  Greek names e.g. "herakles" or Hispanic names e.g. "Robles", where 'e' is pronounced, other exceptions
            if (((((m_current + 1)
                        == m_last)
                        && (StringAt((m_current - 3), 5, "OCLES", "ACLES", "AKLES", "")
                        || (StringAt(0, 4, "INES", "")
                        || (StringAt(0, 5, "LOPES", "ESTES", "GOMES", "NUNES", "ALVES", "ICKES", "INNES", "PERES", "WAGES", "NEVES", "BENES", "DONES", "")
                        || (StringAt(0, 6, "CORTES", "CHAVES", "VALDES", "ROBLES", "TORRES", "FLORES", "BORGES", "NIEVES", "MONTES", "SOARES", "VALLES", "GEDDES", "ANDRES", "VIAJES", "CALLES", "FONTES", "HERMES", "ACEVES", "BATRES", "MATHES", "")
                        || (StringAt(0, 7, "DELORES", "MORALES", "DOLORES", "ANGELES", "ROSALES", "MIRELES", "LINARES", "PERALES", "PAREDES", "BRIONES", "SANCHES", "CAZARES", "REVELES", "ESTEVES", "ALVARES", "MATTHES", "SOLARES", "CASARES", "CACERES", "STURGES", "RAMIRES", "FUNCHES", "BENITES", "FUENTES", "PUENTES", "TABARES", "HENTGES", "VALORES", "")
                        || (StringAt(0, 8, "GONZALES", "MERCEDES", "FAGUNDES", "JOHANNES", "GONSALES", "BERMUDES", "CESPEDES", "BETANCES", "TERRONES", "DIOGENES", "CORRALES", "CABRALES", "MARTINES", "GRAJALES", "")
                        || (StringAt(0, 9, "CERVANTES", "FERNANDES", "GONCALVES", "BENEVIDES", "CIFUENTES", "SIFUENTES", "SERVANTES", "HERNANDES", "BENAVIDES", "") || StringAt(0, 10, "ARCHIMEDES", "CARRIZALES", "MAGALLANES", "")))))))))
                        || (StringAt((m_current - 2), 4, "FRED", "DGES", "DRED", "GNES", "")
                        || (StringAt((m_current - 5), 7, "PROBLEM", "RESPLEN", "")
                        || (StringAt((m_current - 4), 6, "REPLEN", "") || StringAt((m_current - 3), 4, "SPLE", ""))))))
            {
                return true;
            }

            return false;
        }

        private bool Skip_Silent_UE()
        {
            // always silent except for cases listed below
            if ((StringAt((m_current - 1), 3, "QUE", "GUE", "") && !StringAt(0, 8, "BARBEQUE", "PALENQUE", "APPLIQUE", "")
                    // '-que' cases usually French but missing the acute accent
                    && !StringAt(0, 6, "RISQUE", "") && !StringAt((m_current - 3), 5, "ARGUE", "SEGUE", "")
                    && !StringAt(0, 7, "PIROGUE", "ENRIQUE", "") && !StringAt(0, 10, "COMMUNIQUE", "")) && (m_current > 1)
                    && (((m_current + 1) == m_last) || StringAt(0, 7, "JACQUES", "")))
            {
                m_current = SkipVowels(m_current);
                return true;
            }

            return false;
        }

        private void Encode_B()
        {
            if (Encode_Silent_B())
            {
                return;
            }

            // "-MB", e.g", "dumb", already skipped over under
            // 'M', altho it should really be handled here...
            MetaphAddExactApprox("B", "P");

            if ((CharAt(m_current + 1) == 'B') || ((CharAt(m_current + 1) == 'P') && ((m_current + 1 < m_last) && (CharAt(m_current + 2) != 'H'))))
            {
                m_current += 2;
            }
            else
            {
                m_current++;
            }
        }

        private bool Encode_Silent_B()
        {
            // 'debt', 'doubt', 'subtle'
            if ((StringAt((m_current - 2), 4, "DEBT", "")
                        || (StringAt((m_current - 2), 5, "SUBTL", "")
                        || (StringAt((m_current - 2), 6, "SUBTIL", "")
                        || StringAt((m_current - 3), 5, "DOUBT", "")))))
            {
                MetaphAdd("T");
                m_current += 2;
                return true;
            }

            return false;
        }

        private void Encode_C()
        {

            if (Encode_Silent_C_At_Beginning() || Encode_CA_To_S() || Encode_CO_To_S() || Encode_CH() || Encode_CCIA()
                    || Encode_CC() || Encode_CK_CG_CQ() || Encode_C_Front_Vowel() || Encode_Silent_C() || Encode_CZ()
                    || Encode_CS())
            {
                return;
            }

            // else
            if (!StringAt((m_current - 1), 1, "C", "K", "G", "Q", ""))
            {
                MetaphAdd("K");
            }

            // name sent in 'mac caffrey', 'mac Gregor
            if (StringAt((m_current + 1), 2, " C", " Q", " G", ""))
            {
                m_current += 2;
            }
            else
            {
                if (StringAt((m_current + 1), 1, "C", "K", "Q", "") && !StringAt((m_current + 1), 2, "CE", "CI", ""))
                {
                    m_current += 2;
                    // account for combinations such as Ro-ckc-liffe
                    if (StringAt((m_current), 1, "C", "K", "Q", "") && !StringAt((m_current + 1), 2, "CE", "CI", ""))
                    {
                        m_current++;
                    }
                }
                else
                {
                    m_current++;
                }
            }
        }

        private bool Encode_Silent_C_At_Beginning()
        {
            // skip these when at start of word
            if (((m_current == 0)
                        && StringAt(m_current, 2, "CT", "CN", "")))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_CA_To_S()
        {
            //  Special case: 'Caesar'. 
            //  Also, where cedilla not used, as in "linguica" => LNKS
            if ((((m_current == 0)
                        && StringAt(m_current, 4, "CAES", "CAEC", "CAEM", ""))
                        || (StringAt(0, 8, "FRANCAIS", "FRANCAIX", "LINGUICA", "")
                        || (StringAt(0, 6, "FACADE", "") || StringAt(0, 9, "GONCALVES", "PROVENCAL", "")))))
            {
                MetaphAdd("S");
                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_CO_To_S()
        {
            //  e.g. 'coelecanth' => SLKN0
            if (((StringAt(m_current, 4, "COEL", "")
                        && (IsVowel((m_current + 4))
                        || ((m_current + 3)
                        == m_last)))
                        || (StringAt(m_current, 5, "COENA", "COENO", "")
                        || (StringAt(0, 8, "FRANCOIS", "MELANCON", "") || StringAt(0, 6, "GARCON", "")))))
            {
                MetaphAdd("S");
                AdvanceCounter(3, 1);
                return true;
            }

            return false;
        }

        private bool Encode_CH()
        {
            if (StringAt(m_current, 2, "CH", ""))
            {
                if (Encode_CHAE() || Encode_CH_To_H() || Encode_Silent_CH() || Encode_ARCH()
                        // Encode_CH_To_X() should be
                        // called before the Germanic
                        // and Greek encoding functions
                        || Encode_CH_To_X() || Encode_English_CH_To_K() || Encode_Germanic_CH_To_K()
                        || Encode_Greek_CH_Initial() || Encode_Greek_CH_Non_Initial())
                {
                    return true;
                }

                if (m_current > 0)
                {
                    if (StringAt(0, 2, "MC", "") && (m_current == 1))
                    {
                        // e.g., "McHugh"
                        MetaphAdd("K");
                    }
                    else
                    {
                        MetaphAdd("X", "K");
                    }
                }
                else
                {
                    MetaphAdd("X");
                }
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_CHAE()
        {
            //  e.g. 'Michael'
            if (((m_current > 0)
                        && StringAt((m_current + 2), 2, "AE", "")))
            {
                if (StringAt(0, 7, "RACHAEL", ""))
                {
                    MetaphAdd("X");
                }
                else if (!StringAt((m_current - 1), 1, "C", "K", "G", "Q", ""))
                {
                    MetaphAdd("K");
                }

                AdvanceCounter(4, 2);
                return true;
            }

            return false;
        }

        private bool Encode_CH_To_H()
        {
            //  Hebrew => 'H', e.g. 'channukah', 'chabad'
            if ((((m_current == 0)
                        && (StringAt((m_current + 2), 3, "AIM", "ETH", "ELM", "")
                        || (StringAt((m_current + 2), 4, "ASID", "AZAN", "")
                        || (StringAt((m_current + 2), 5, "UPPAH", "UTZPA", "ALLAH", "ALUTZ", "AMETZ", "")
                        || (StringAt((m_current + 2), 6, "ESHVAN", "ADARIM", "ANUKAH", "") || StringAt((m_current + 2), 7, "ALLLOTH", "ANNUKAH", "AROSETH", ""))))))
                        || StringAt((m_current - 3), 7, "CLACHAN", "")))
            {
                MetaphAdd("H");
                AdvanceCounter(3, 2);
                return true;
            }

            return false;
        }

        private bool Encode_Silent_CH()
        {
            //  '-ch-' not pronounced
            if ((StringAt((m_current - 2), 7, "FUCHSIA", "")
                        || (StringAt((m_current - 2), 5, "YACHT", "")
                        || (StringAt(0, 8, "STRACHAN", "")
                        || (StringAt(0, 8, "CRICHTON", "")
                        || (StringAt((m_current - 3), 6, "DRACHM", "")
                        && !StringAt((m_current - 3), 7, "DRACHMA", "")))))))
            {
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_CH_To_X()
        {
            //  e.g. 'approach', 'beach'
            if (((StringAt((m_current - 2), 4, "OACH", "EACH", "EECH", "OUCH", "OOCH", "MUCH", "SUCH", "")
                        && !StringAt((m_current - 3), 5, "JOACH", ""))
                        || ((((m_current + 2)
                        == m_last)
                        && StringAt((m_current - 1), 4, "ACHA", "ACHO", ""))
                        || ((StringAt(m_current, 4, "CHOT", "CHOD", "CHAT", "")
                        && ((m_current + 3)
                        == m_last))
                        || (((StringAt((m_current - 1), 4, "OCHE", "")
                        && ((m_current + 2)
                        == m_last))
                        && !StringAt((m_current - 2), 5, "DOCHE", ""))
                        || (StringAt((m_current - 4), 6, "ATTACH", "DETACH", "KOVACH", "")
                        || (StringAt((m_current - 5), 7, "SPINACH", "")
                        || (StringAt(0, 6, "MACHAU", "")
                        || (StringAt((m_current - 4), 8, "PARACHUT", "")
                        || (StringAt((m_current - 5), 8, "MASSACHU", "")
                        || ((StringAt((m_current - 3), 5, "THACH", "")
                        && !StringAt((m_current - 1), 4, "ACHE", ""))
                        || StringAt((m_current - 2), 6, "VACHON", ""))))))))))))
            {
                MetaphAdd("X");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_English_CH_To_K()
        {
            // 'ache', 'echo', alternate spelling of 'Michael'
            if ((((m_current == 1)
                        && RootOrInflections(m_inWord, "ACHE"))
                        || ((((m_current > 3)
                        && RootOrInflections(m_inWord.Substring((m_current - 1)), "ACHE"))
                        && (StringAt(0, 3, "EAR", "")
                        || (StringAt(0, 4, "HEAD", "BACK", "") || StringAt(0, 5, "HEART", "BELLY", "TOOTH", ""))))
                        || (StringAt((m_current - 1), 4, "ECHO", "")
                        || (StringAt((m_current - 2), 7, "MICHEAL", "")
                        || (StringAt((m_current - 4), 7, "JERICHO", "") || StringAt((m_current - 5), 7, "LEPRECH", "")))))))
            {
                MetaphAdd("K", "X");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_Germanic_CH_To_K()
        {
            // various Germanic
            // "<consonant><vowel>CH-"implies a German word where 'ch' => K
            if (((m_current > 1) && !IsVowel(m_current - 2) && StringAt((m_current - 1), 3, "ACH", "")
                    && !StringAt((m_current - 2), 7, "MACHADO", "MACHUCA", "LACHANC", "LACHAPE", "KACHATU", "")
                    && !StringAt((m_current - 3), 7, "KHACHAT", "")
                    && ((CharAt(m_current + 2) != 'I') && ((CharAt(m_current + 2) != 'E')
                            || StringAt((m_current - 2), 6, "BACHER", "MACHER", "MACHEN", "LACHER", "")))
                    // e.g. 'Brecht', 'Fuchs'
                    || (StringAt((m_current + 2), 1, "T", "S", "")
                            && !(StringAt(0, 11, "WHICHSOEVER", "") || StringAt(0, 9, "LUNCHTIME", "")))
                    // e.g. 'Andromache'
                    || StringAt(0, 4, "SCHR", "") || ((m_current > 2) && StringAt((m_current - 2), 5, "MACHE", ""))
                    || ((m_current == 2) && StringAt((m_current - 2), 4, "ZACH", ""))
                    || StringAt((m_current - 4), 6, "SCHACH", "") || StringAt((m_current - 1), 5, "ACHEN", "")
                    || StringAt((m_current - 3), 5, "SPICH", "ZURCH", "BUECH", "")
                    || (StringAt((m_current - 3), 5, "KIRCH", "JOACH", "BLECH", "MALCH", "")
                            // "kirch" and "blech" both get 'X'
                            && !(StringAt((m_current - 3), 8, "KIRCHNER", "") || ((m_current + 1) == m_last)))
                    || (((m_current + 1) == m_last) && StringAt((m_current - 2), 4, "NICH", "LICH", "BACH", ""))
                    || (((m_current + 1) == m_last)
                            && StringAt((m_current - 3), 5, "URICH", "BRICH", "ERICH", "DRICH", "NRICH", "")
                            && !StringAt((m_current - 5), 7, "ALDRICH", "") && !StringAt((m_current - 6), 8, "GOODRICH", "")
                            && !StringAt((m_current - 7), 9, "GINGERICH", "")))
                    || (((m_current + 1) == m_last)
                            && StringAt((m_current - 4), 6, "ULRICH", "LFRICH", "LLRICH", "EMRICH", "ZURICH", "EYRICH", ""))
                    // e.g., 'wachtler', 'wechsler', but not 'tichner'
                    || ((StringAt((m_current - 1), 1, "A", "O", "U", "E", "") || (m_current == 0))
                            && StringAt((m_current + 2), 1, "L", "R", "N", "M", "B", "H", "F", "V", "W", " ", "")))
            {
                // "CHR/L-" e.g. 'Chris' do not get
                // alt pronunciation of 'X'
                if (StringAt((m_current + 2), 1, "R", "L", "") || SlavoGermanic())
                {
                    MetaphAdd("K");
                }
                else
                {
                    MetaphAdd("K", "X");
                }
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_ARCH()
        {
            if (StringAt((m_current - 2), 4, "ARCH", ""))
            {
                //  "-ARCH-" has many combining forms where "-CH-" => K because of its
                //  derivation from the Greek
                if ((((IsVowel((m_current + 2)) && StringAt((m_current - 2), 5, "ARCHA", "ARCHI", "ARCHO", "ARCHU", "ARCHY", ""))
                            || (StringAt((m_current - 2), 6, "ARCHEA", "ARCHEG", "ARCHEO", "ARCHET", "ARCHEL", "ARCHES", "ARCHEP", "ARCHEM", "ARCHEN", "")
                            || ((StringAt((m_current - 2), 4, "ARCH", "")
                            && ((m_current + 1)
                            == m_last))
                            || StringAt(0, 7, "MENARCH", ""))))
                            && (!RootOrInflections(m_inWord, "ARCH")
                            && (!StringAt((m_current - 4), 6, "SEARCH", "POARCH", "")
                            && (!StringAt(0, 9, "ARCHENEMY", "ARCHIBALD", "ARCHULETA", "ARCHAMBAU", "")
                            && (!StringAt(0, 6, "ARCHER", "ARCHIE", "")
                            && !((((StringAt((m_current - 3), 5, "LARCH", "MARCH", "PARCH", "") || StringAt((m_current - 4), 6, "STARCH", ""))
                            && !(StringAt(0, 6, "EPARCH", "")
                            || (StringAt(0, 7, "NOMARCH", "")
                            || (StringAt(0, 8, "EXILARCH", "HIPPARCH", "MARCHESE", "")
                            || (StringAt(0, 9, "ARISTARCH", "") || StringAt(0, 9, "MARCHETTI", ""))))))
                            || RootOrInflections(m_inWord, "STARCH"))
                            && (!StringAt((m_current - 2), 5, "ARCHU", "ARCHY", "")
                            || StringAt(0, 7, "STARCHY", "")))))))))
                {
                    MetaphAdd("K", "X");
                }
                else
                {
                    MetaphAdd("X");
                }

                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_Greek_CH_Initial()
        {
            // Greek roots e.g. 'chemistry', 'chorus', ch at beginning of root
            if ((StringAt(m_current, 6, "CHAMOM", "CHARAC", "CHARIS", "CHARTO", "CHARTU", "CHARYB", "CHRIST", "CHEMIC",
                    "CHILIA", "")
                    || (StringAt(m_current, 5, "CHEMI", "CHEMO", "CHEMU", "CHEMY", "CHOND", "CHONA", "CHONI", "CHOIR",
                            "CHASM", "CHARO", "CHROM", "CHROI", "CHAMA", "CHALC", "CHALD", "CHAET", "CHIRO", "CHILO",
                            "CHELA", "CHOUS", "CHEIL", "CHEIR", "CHEIM", "CHITI", "CHEOP", "")
                            && !(StringAt(m_current, 6, "CHEMIN", "") || StringAt((m_current - 2), 8, "ANCHONDO", "")))
                    || (StringAt(m_current, 5, "CHISM", "CHELI", "")
                            // exclude Spanish "machismo"
                            && !(StringAt(0, 8, "MACHISMO", "")
                                    // exclude some French words
                                    || StringAt(0, 10, "REVANCHISM", "") || StringAt(0, 9, "RICHELIEU", "")
                                    || (StringAt(0, 5, "CHISM", "") && (m_length == 5)) || StringAt(0, 6, "MICHEL", "")))
                    // include e.g. "chorus", "chyme", "chaos"
                    || (StringAt(m_current, 4, "CHOR", "CHOL", "CHYM", "CHYL", "CHLO", "CHOS", "CHUS", "CHOE", "")
                            && !StringAt(0, 6, "CHOLLO", "CHOLLA", "CHORIZ", ""))
                    // "chaos" => K but not "Chao"
                    || (StringAt(m_current, 4, "CHAO", "") && ((m_current + 3) != m_last))
                    // e.g. "abranchiate"
                    || (StringAt(m_current, 4, "CHIA", "")
                            && !(StringAt(0, 10, "APPALACHIA", "") || StringAt(0, 7, "CHIAPAS", "")))
                    // e.g. "chimera"
                    || StringAt(m_current, 7, "CHIMERA", "CHIMAER", "CHIMERI", "")
                    // e.g. "chameleon"
                    || ((m_current == 0) && StringAt(m_current, 5, "CHAME", "CHELO", "CHITO", ""))
                    // e.g. "spirochete"
                    || ((((m_current + 4) == m_last) || ((m_current + 5) == m_last))
                            && StringAt((m_current - 1), 6, "OCHETE", "")))
                    // more exceptions where "-CH-" => X e.g. "chortle", "crocheter"
                    && !((StringAt(0, 5, "CHORE", "CHOLO", "CHOLA", "") && (m_length == 5))
                            || StringAt(m_current, 5, "CHORT", "CHOSE", "") || StringAt((m_current - 3), 7, "CROCHET", "")
                            || StringAt(0, 7, "CHEMISE", "CHARISE", "CHARISS", "CHAROLE", "")))
            {
                // "CHR/L-" e.g. 'Christ', 'chlorine' do not get
                // alt pronunciation of 'X'
                if (StringAt((m_current + 2), 1, "R", "L", ""))
                {
                    MetaphAdd("K");
                }
                else
                {
                    MetaphAdd("K", "X");
                }
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_Greek_CH_Non_Initial()
        {
            // Greek & other roots e.g. 'tachometer', 'orchid', ch in middle or end of root
            if (StringAt((m_current - 2), 6, "ORCHID", "NICHOL", "MECHAN", "LICHEN", "MACHIC", "PACHEL", "RACHIF", "RACHID",
                    "RACHIS", "RACHIC", "MICHAL", "")
                    || StringAt((m_current - 3), 5, "MELCH", "GLOCH", "TRACH", "TROCH", "BRACH", "SYNCH", "PSYCH", "STICH",
                            "PULCH", "EPOCH", "")
                    || (StringAt((m_current - 3), 5, "TRICH", "") && !StringAt((m_current - 5), 7, "OSTRICH", ""))
                    || (StringAt((m_current - 2), 4, "TYCH", "TOCH", "BUCH", "MOCH", "CICH", "DICH", "NUCH", "EICH", "LOCH",
                            "DOCH", "ZECH", "WYCH", "")
                            && !(StringAt((m_current - 4), 9, "INDOCHINA", "")
                                    || StringAt((m_current - 2), 6, "BUCHON", "")))
                    || StringAt((m_current - 2), 5, "LYCHN", "TACHO", "ORCHO", "ORCHI", "LICHO", "")
                    || (StringAt((m_current - 1), 5, "OCHER", "ECHIN", "ECHID", "")
                            && ((m_current == 1) || (m_current == 2)))
                    || StringAt((m_current - 4), 6, "BRONCH", "STOICH", "STRYCH", "TELECH", "PLANCH", "CATECH", "MANICH",
                            "MALACH", "BIANCH", "DIDACH", "")
                    || (StringAt((m_current - 1), 4, "ICHA", "ICHN", "") && (m_current == 1))
                    || StringAt((m_current - 2), 8, "ORCHESTR", "")
                    || StringAt((m_current - 4), 8, "BRANCHIO", "BRANCHIF", "")
                    || (StringAt((m_current - 1), 5, "ACHAB", "ACHAD", "ACHAN", "ACHAZ", "")
                            && !StringAt((m_current - 2), 7, "MACHADO", "LACHANC", ""))
                    || StringAt((m_current - 1), 6, "ACHISH", "ACHILL", "ACHAIA", "ACHENE", "")
                    || StringAt((m_current - 1), 7, "ACHAIAN", "ACHATES", "ACHIRAL", "ACHERON", "")
                    || StringAt((m_current - 1), 8, "ACHILLEA", "ACHIMAAS", "ACHILARY", "ACHELOUS", "ACHENIAL", "ACHERNAR",
                            "")
                    || StringAt((m_current - 1), 9, "ACHALASIA", "ACHILLEAN", "ACHIMENES", "")
                    || StringAt((m_current - 1), 10, "ACHIMELECH", "ACHITOPHEL", "")
                    // e.g. 'inchoate'
                    || (((m_current - 2) == 0) && (StringAt((m_current - 2), 6, "INCHOA", "")
                            // e.g. 'ischemia'
                            || StringAt(0, 4, "ISCH", "")))
                    // e.g. 'ablimelech', 'Antioch', 'Pentateuch'
                    || (((m_current + 1) == m_last) && StringAt((m_current - 1), 1, "A", "O", "U", "E", "")
                            && !(StringAt(0, 7, "DEBAUCH", "") || StringAt((m_current - 2), 4, "MUCH", "SUCH", "KOCH", "")
                                    || StringAt((m_current - 5), 7, "OODRICH", "ALDRICH", ""))))
            {
                MetaphAdd("K", "X");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_CCIA()
        {
            // e.g., 'focaccia'
            if (StringAt((m_current + 1), 3, "CIA", ""))
            {
                MetaphAdd("X", "S");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_CC()
        {
            // double 'C', but not if e.g. 'McClellan'
            if (StringAt(m_current, 2, "CC", "") && !((m_current == 1) && (CharAt(0) == 'M')))
            {
                // exception
                if (StringAt((m_current - 3), 7, "FLACCID", ""))
                {
                    MetaphAdd("S");
                    AdvanceCounter(3, 2);
                    return true;
                }

                // 'bacci', 'bertucci', other Italian
                if ((((m_current + 2) == m_last) && StringAt((m_current + 2), 1, "I", ""))
                        || StringAt((m_current + 2), 2, "IO", "")
                        || (((m_current + 4) == m_last) && StringAt((m_current + 2), 3, "INO", "INI", "")))
                {
                    MetaphAdd("X");
                    AdvanceCounter(3, 2);
                    return true;
                }

                // 'accident', 'accede' 'succeed'
                if (StringAt((m_current + 2), 1, "I", "E", "Y", "")
                        // except 'bellocchio','Bacchus', 'soccer' get K
                        && !((CharAt(m_current + 2) == 'H') || StringAt((m_current - 2), 6, "SOCCER", "")))
                {
                    MetaphAdd("KS");
                    AdvanceCounter(3, 2);
                    return true;

                }
                else
                {
                    // Pierce's rule
                    MetaphAdd("K");
                    m_current += 2;
                    return true;
                }
            }

            return false;
        }


        private bool Encode_CK_CG_CQ()
        {
            if (StringAt(m_current, 2, "CK", "CG", "CQ", ""))
            {
                //  eastern European spelling e.g. 'gorecki' == 'goresky'
                if ((StringAt(m_current, 3, "CKI", "CKY", "")
                            && (((m_current + 2)
                            == m_last)
                            && (m_length > 6))))
                {
                    MetaphAdd("K", "SK");
                }
                else
                {
                    MetaphAdd("K");
                }

                m_current += 2;
                if (StringAt(m_current, 1, "K", "G", "Q", ""))
                {
                    m_current++;
                }

                return true;
            }

            return false;
        }

        private bool Encode_C_Front_Vowel()
        {
            if (StringAt(m_current, 2, "CI", "CE", "CY", ""))
            {
                if ((Encode_British_Silent_CE()
                            || (Encode_CE()
                            || (Encode_CI() || Encode_Latinate_Suffixes()))))
                {
                    AdvanceCounter(2, 1);
                    return true;
                }

                MetaphAdd("S");
                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_British_Silent_CE()
        {
            //  English place names like e.g.'gloucester' pronounced glo-ster
            if (((StringAt((m_current + 1), 5, "ESTER", "")
                        && ((m_current + 5)
                        == m_last))
                        || StringAt((m_current + 1), 10, "ESTERSHIRE", "")))
            {
                return true;
            }

            return false;
        }

        private bool Encode_CE()
        {
            // 'ocean', 'commercial', 'provincial', 'cello', 'fettucini', 'Medici'
            if ((StringAt((m_current + 1), 3, "EAN", "") && IsVowel(m_current - 1))
                    // e.g. 'rosacea'
                    || (StringAt((m_current - 1), 4, "ACEA", "") && ((m_current + 2) == m_last)
                            && !StringAt(0, 7, "PANACEA", ""))
                    // e.g. 'Botticelli', 'concerto'
                    || StringAt((m_current + 1), 4, "ELLI", "ERTO", "EORL", "")
                    // some Italian names familiar to Americans
                    || (StringAt((m_current - 3), 5, "CROCE", "") && ((m_current + 1) == m_last))
                    || StringAt((m_current - 3), 5, "DOLCE", "")
                    // e.g. 'cello'
                    || (StringAt((m_current + 1), 4, "ELLO", "") && ((m_current + 4) == m_last)))
            {
                MetaphAdd("X", "S");
                return true;
            }

            return false;
        }

        private bool Encode_CI()
        {
            // with consonant before C
            // e.g. 'fettucini', but exception for the americanized pronunciation of
            // 'Mancini'
            if (((StringAt((m_current + 1), 3, "INI", "") && !StringAt(0, 7, "MANCINI", "")) && ((m_current + 3) == m_last))
                    // e.g. 'Medici'
                    || (StringAt((m_current - 1), 3, "ICI", "") && ((m_current + 1) == m_last))
                    // e.g. "commercial', 'provincial', 'cistercian'
                    || StringAt((m_current - 1), 5, "RCIAL", "NCIAL", "RCIAN", "UCIUS", "")
                    // special cases
                    || StringAt((m_current - 3), 6, "MARCIA", "") || StringAt((m_current - 2), 7, "ANCIENT", ""))
            {
                MetaphAdd("X", "S");
                return true;
            }

            // with vowel before C (or at beginning?)
            if (((StringAt(m_current, 3, "CIO", "CIE", "CIA", "") && IsVowel(m_current - 1))
                    // e.g. "ciao"
                    || StringAt((m_current + 1), 3, "IAO", "")) && !StringAt((m_current - 4), 8, "COERCION", ""))
            {
                if ((StringAt(m_current, 4, "CIAN", "CIAL", "CIAO", "CIES", "CIOL", "CION", "")
                        // exception - "glacier" => 'X' but "spacier" = > 'S'
                        || StringAt((m_current - 3), 7, "GLACIER", "")
                        || StringAt(m_current, 5, "CIENT", "CIENC", "CIOUS", "CIATE", "CIATI", "CIATO", "CIABL", "CIARY",
                                "")
                        || (((m_current + 2) == m_last) && StringAt(m_current, 3, "CIA", "CIO", ""))
                        || (((m_current + 3) == m_last) && StringAt(m_current, 4, "CIAS", "CIOS", "")))
                        // exceptions
                        && !(StringAt((m_current - 4), 11, "ASSOCIATION", "") || StringAt(0, 4, "OCIE", "")
                                // exceptions mostly because these names are usually from
                                // the Spanish rather than the Italian in America
                                || StringAt((m_current - 2), 5, "LUCIO", "") || StringAt((m_current - 2), 6, "MACIAS", "")
                                || StringAt((m_current - 3), 6, "GRACIE", "GRACIA", "")
                                || StringAt((m_current - 2), 7, "LUCIANO", "")
                                || StringAt((m_current - 3), 8, "MARCIANO", "")
                                || StringAt((m_current - 4), 7, "PALACIO", "")
                                || StringAt((m_current - 4), 9, "FELICIANO", "")
                                || StringAt((m_current - 5), 8, "MAURICIO", "")
                                || StringAt((m_current - 7), 11, "ENCARNACION", "")
                                || StringAt((m_current - 4), 8, "POLICIES", "")
                                || StringAt((m_current - 2), 8, "HACIENDA", "")
                                || StringAt((m_current - 6), 9, "ANDALUCIA", "")
                                || StringAt((m_current - 2), 5, "SOCIO", "SOCIE", "")))
                {
                    MetaphAdd("X", "S");
                }
                else
                {
                    MetaphAdd("S", "X");
                }

                return true;
            }

            // exception
            if (StringAt((m_current - 4), 8, "COERCION", ""))
            {
                MetaphAdd("J");
                return true;
            }

            return false;
        }

        private bool Encode_Latinate_Suffixes()
        {
            if (StringAt((m_current + 1), 4, "EOUS", "IOUS", ""))
            {
                MetaphAdd("X", "S");
                return true;
            }

            return false;
        }

        private bool Encode_Silent_C()
        {
            if (StringAt((m_current + 1), 1, "T", "S", ""))
            {
                if ((StringAt(0, 11, "CONNECTICUT", "") || StringAt(0, 6, "INDICT", "TUCSON", "")))
                {
                    m_current++;
                    return true;
                }

            }

            return false;
        }

        private bool Encode_CZ()
        {
            if ((StringAt((m_current + 1), 1, "Z", "")
                        && !StringAt((m_current - 1), 6, "ECZEMA", "")))
            {
                if (StringAt(m_current, 4, "CZAR", ""))
                {
                    MetaphAdd("S");
                }

                //  otherwise most likely a Czech word...
                MetaphAdd("X");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_CS()
        {
            //  give an 'etymological' 2nd
            //  encoding for "Kovacs" so
            //  that it matches "kovach"
            if (StringAt(0, 6, "KOVACS", ""))
            {
                MetaphAdd("KS", "X");
                m_current += 2;
                return true;
            }

            if ((StringAt((m_current - 1), 3, "ACS", "")
                        && (((m_current + 1)
                        == m_last)
                        && !StringAt((m_current - 4), 6, "ISAACS", ""))))
            {
                MetaphAdd("X");
                m_current += 2;
                return true;
            }

            return false;
        }

        private void Encode_D()
        {
            if ((Encode_DG()
                        || (Encode_DJ()
                        || (Encode_DT_DD()
                        || (Encode_D_To_J()
                        || (Encode_DOUS() || Encode_Silent_D()))))))
            {
                return;
            }

            if (m_encodeExact)
            {
                //  "final de-voicing" in this case
                //  e.g. 'missed' == 'mist'
                if (((m_current == m_last)
                            && StringAt((m_current - 3), 4, "SSED", "")))
                {
                    MetaphAdd("T");
                }
                else
                {
                    MetaphAdd("D");
                }

            }
            else
            {
                MetaphAdd("T");
            }

            m_current++;
        }

        private bool Encode_DG()
        {
            if (StringAt(m_current, 2, "DG", ""))
            {
                //  excludes exceptions e.g. 'Edgar', 
                //  or cases where 'g' is first letter of combining form 
                //  e.g. 'handgun', 'waldglas'
                if (StringAt((m_current + 2), 1, "A", "O", ""))
                {
                    //  e.g. "midgut"
                }

                StringAt((m_current + 1), 3, "GUN", "GUT", "");
                //  e.g. "handgrip"
                StringAt((m_current + 1), 4, "GEAR", "GLAS", "GRIP", "GREN", "GILL", "GRAF", "");
                //  e.g. "mudgard"
                StringAt((m_current + 1), 5, "GUARD", "GUILT", "GRAVE", "GRASS", "");
                //  e.g. "woodgrouse"
                StringAt((m_current + 1), 6, "GROUSE", "");
                MetaphAddExactApprox("DG", "TK");
                // e.g. "edge", "abridgment"
                MetaphAdd("J");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_DJ()
        {
            //  e.g. "adjacent"
            if (StringAt(m_current, 2, "DJ", ""))
            {
                MetaphAdd("J");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_DT_DD()
        {
            //  eat redundant 'T' or 'D'
            if (StringAt(m_current, 2, "DT", "DD", ""))
            {
                if (StringAt(m_current, 3, "DTH", ""))
                {
                    MetaphAddExactApprox("D0", "T0");
                    m_current += 3;
                }
                else
                {
                    if (m_encodeExact)
                    {
                        //  devoice it
                        if (StringAt(m_current, 2, "DT", ""))
                        {
                            MetaphAdd("T");
                        }
                        else
                        {
                            MetaphAdd("D");
                        }

                    }
                    else
                    {
                        MetaphAdd("T");
                    }

                    m_current += 2;
                }

                return true;
            }

            return false;
        }

        private bool Encode_D_To_J()
        {
            // e.g. "module", "adulate"
            if ((StringAt(m_current, 3, "DUL", "") && (IsVowel(m_current - 1) && IsVowel(m_current + 3)))
                    // e.g. "soldier", "grandeur", "procedure"
                    || (((m_current + 3) == m_last) && StringAt((m_current - 1), 5, "LDIER", "NDEUR", "EDURE", "RDURE", ""))
                    || StringAt((m_current - 3), 7, "CORDIAL", "")
                    // e.g. "pendulum", "education"
                    || StringAt((m_current - 1), 5, "NDULA", "NDULU", "EDUCA", "")
                    // e.g. "individual", "individual", "residuum"
                    || StringAt((m_current - 1), 4, "ADUA", "IDUA", "IDUU", ""))
            {
                MetaphAddExactApprox("J", "D", "J", "T");
                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_DOUS()
        {
            //  e.g. "assiduous", "arduous"
            if (StringAt((m_current + 1), 4, "UOUS", ""))
            {
                MetaphAddExactApprox("J", "D", "J", "T");
                AdvanceCounter(4, 1);
                return true;
            }

            return false;
        }

        private bool Encode_Silent_D()
        {
            // silent 'D' e.g. 'Wednesday', 'handsome'
            if (StringAt((m_current - 2), 9, "WEDNESDAY", "")
                    || StringAt((m_current - 3), 7, "HANDKER", "HANDSOM", "WINDSOR", "")
                    // French silent D at end in words or names familiar to Americans
                    || (m_current == m_last && (StringAt((m_current - 5), 6, "PERNOD", "ARTAUD", "RENAUD", "")
                    || StringAt((m_current - 6), 7, "RIMBAUD", "MICHAUD", "BICHAUD", ""))))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private void Encode_F()
        {
            //  Encode cases where "-FT-" => "T" is usually silent
            //  e.g. 'often', 'soften'
            //  This should really be covered under "T"!
            if (StringAt((m_current - 1), 5, "OFTEN", ""))
            {
                MetaphAdd("F", "FT");
                m_current += 2;
                return;
            }

            //  eat redundant 'F'
            if ((CharAt((m_current + 1)) == 'F'))
            {
                m_current += 2;
            }
            else
            {
                m_current++;
            }

            MetaphAdd("F");
        }

        private void Encode_G()
        {
            if ((Encode_Silent_G_At_Beginning()
                        || (Encode_GG()
                        || (Encode_GK()
                        || (Encode_GH()
                        || (Encode_Silent_G()
                        || (Encode_GN()
                        || (Encode_GL()
                        || (Encode_Initial_G_Front_Vowel()
                        || (Encode_NGER()
                        || (Encode_GER()
                        || (Encode_GEL()
                        || (Encode_Non_Initial_G_Front_Vowel() || Encode_GA_To_J())))))))))))))
            {
                return;
            }

            if (!StringAt((m_current - 1), 1, "C", "K", "G", "Q", ""))
            {
                MetaphAddExactApprox("G", "K");
            }

            m_current++;
        }

        private bool Encode_Silent_G_At_Beginning()
        {
            // skip these when at start of word
            if (((m_current == 0)
                        && StringAt(m_current, 2, "GN", "")))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_GG()
        {
            if (CharAt(m_current + 1) == 'G')
            {
                // Italian e.g, 'loggia', 'Caraveggio', also 'suggest' and 'exaggerate'
                if (StringAt((m_current - 1), 5, "AGGIA", "OGGIA", "AGGIO", "EGGIO", "EGGIA", "IGGIO", "")
                        // 'Ruggiero' but not 'snuggies'
                        || (StringAt((m_current - 1), 5, "UGGIE", "")
                                && !(((m_current + 3) == m_last) || ((m_current + 4) == m_last)))
                        || (((m_current + 2) == m_last) && StringAt((m_current - 1), 4, "AGGI", "OGGI", ""))
                        || StringAt((m_current - 2), 6, "SUGGES", "XAGGER", "REGGIE", ""))
                {
                    // expecting where "-GG-" => KJ
                    if (StringAt((m_current - 2), 7, "SUGGEST", ""))
                    {
                        MetaphAddExactApprox("G", "K");
                    }

                    MetaphAdd("J");
                    AdvanceCounter(3, 2);
                }
                else
                {
                    MetaphAddExactApprox("G", "K");
                    m_current += 2;
                }
                return true;
            }

            return false;
        }

        private bool Encode_GK()
        {
            //  'ginkgo'
            if ((CharAt((m_current + 1)) == 'K'))
            {
                MetaphAdd("K");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_GH()
        {
            if ((CharAt((m_current + 1)) == 'H'))
            {
                if ((Encode_GH_After_Consonant()
                            || (Encode_Initial_GH()
                            || (Encode_GH_To_J()
                            || (Encode_GH_To_H()
                            || (Encode_UGHT()
                            || (Encode_GH_H_Part_Of_Other_Word()
                            || (Encode_Silent_GH() || Encode_GH_To_F()))))))))
                {
                    return true;
                }

                MetaphAddExactApprox("G", "K");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_GH_After_Consonant()
        {
            // e.g. 'burgher', 'Bingham'
            if ((m_current > 0) && !IsVowel(m_current - 1)
                    // not e.g. 'greenhalgh'
                    && !(StringAt((m_current - 3), 5, "HALGH", "") && ((m_current + 1) == m_last)))
            {
                MetaphAddExactApprox("G", "K");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_Initial_GH()
        {
            if ((m_current < 3))
            {
                //  e.g. "ghislane", "ghiradelli"
                if ((m_current == 0))
                {
                    if ((CharAt((m_current + 2)) == 'I'))
                    {
                        MetaphAdd("J");
                    }
                    else
                    {
                        MetaphAddExactApprox("G", "K");
                    }

                    m_current += 2;
                    return true;
                }

            }

            return false;
        }

        private bool Encode_GH_To_J()
        {
            //  e.g., 'greenhalgh', 'dunkenhalgh', English names
            if ((StringAt((m_current - 2), 4, "ALGH", "")
                        && ((m_current + 1)
                        == m_last)))
            {
                MetaphAdd("J", "");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_GH_To_H()
        {
            //  special cases
            //  e.g., 'donoghue', 'donaghy'
            if (((StringAt((m_current - 4), 4, "DONO", "DONA", "") && IsVowel((m_current + 2)))
                        || StringAt((m_current - 5), 9, "CALLAGHAN", "")))
            {
                MetaphAdd("H");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_UGHT()
        {
            // e.g. "ought", "aught", "daughter", "slaughter"    
            if (StringAt((m_current - 1), 4, "UGHT", ""))
            {
                if (((StringAt((m_current - 3), 5, "LAUGH", "")
                            && !(StringAt((m_current - 4), 7, "SLAUGHT", "") || StringAt((m_current - 3), 7, "LAUGHTO", "")))
                            || StringAt((m_current - 4), 6, "DRAUGH", "")))
                {
                    MetaphAdd("FT");
                }
                else
                {
                    MetaphAdd("T");
                }

                m_current += 3;
                return true;
            }

            return false;
        }

        private bool Encode_GH_H_Part_Of_Other_Word()
        {
            //  if the 'H' is the beginning of another word or syllable
            if (StringAt((m_current + 1), 4, "HOUS", "HEAD", "HOLE", "HORN", "HARN", ""))
            {
                MetaphAddExactApprox("G", "K");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_Silent_GH()
        {
            // Parker's rule (with some further refinements) - e.g., 'hugh'
            if (((((m_current > 1) && StringAt((m_current - 2), 1, "B", "H", "D", "G", "L", ""))
                    // e.g., 'bough'
                    || ((m_current > 2) && StringAt((m_current - 3), 1, "B", "H", "D", "K", "W", "N", "P", "V", "")
                            && !StringAt(0, 6, "ENOUGH", ""))
                    // e.g., 'broughton'
                    || ((m_current > 3) && StringAt((m_current - 4), 1, "B", "H", ""))
                    // 'plough', 'slaugh'
                    || ((m_current > 3) && StringAt((m_current - 4), 2, "PL", "SL", "")) || ((m_current > 0)
                            // 'sigh', 'light'
                            && ((CharAt(m_current - 1) == 'I') || StringAt(0, 4, "PUGH", "")
                                    // e.g. 'MCDONAGH', 'MURTAGH', 'CREAGH'
                                    || (StringAt((m_current - 1), 3, "AGH", "") && ((m_current + 1) == m_last))
                                    || StringAt((m_current - 4), 6, "GERAGH", "DRAUGH", "")
                                    || (StringAt((m_current - 3), 5, "GAUGH", "GEOGH", "MAUGH", "")
                                            && !StringAt(0, 9, "MCGAUGHEY", ""))
                                    // exceptions to 'tough', 'rough', 'lough'
                                    || (StringAt((m_current - 2), 4, "OUGH", "") && (m_current > 3)
                                            && !StringAt((m_current - 4), 6, "CCOUGH", "ENOUGH", "TROUGH", "CLOUGH", "")))))
                    // suffixes starting w/ vowel where "-GH-" is usually silent
                    && (StringAt((m_current - 3), 5, "VAUGH", "FEIGH", "LEIGH", "")
                            || StringAt((m_current - 2), 4, "HIGH", "TIGH", "") || ((m_current + 1) == m_last)
                            || (StringAt((m_current + 2), 2, "IE", "EY", "ES", "ER", "ED", "TY", "")
                                    && ((m_current + 3) == m_last) && !StringAt((m_current - 5), 9, "GALLAGHER", ""))
                            || (StringAt((m_current + 2), 1, "Y", "") && ((m_current + 2) == m_last))
                            || (StringAt((m_current + 2), 3, "ING", "OUT", "") && ((m_current + 4) == m_last))
                            || (StringAt((m_current + 2), 4, "ERTY", "") && ((m_current + 5) == m_last))
                            || (!IsVowel(m_current + 2) || StringAt((m_current - 3), 5, "GAUGH", "GEOGH", "MAUGH", "")
                                    || StringAt((m_current - 4), 8, "BROUGHAM", ""))))
                    // exceptions where '-g-' pronounced
                    && !(StringAt(0, 6, "BALOGH", "SABAGH", "") || StringAt((m_current - 2), 7, "BAGHDAD", "")
                            || StringAt((m_current - 3), 5, "WHIGH", "")
                            || StringAt((m_current - 5), 7, "SABBAGH", "AKHLAGH", "")))
            {
                // silent - do nothing
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_GH_Special_Cases()
        {
            bool handled = false;
            //  special case: 'hiccough' == 'hiccup'
            if (StringAt((m_current - 6), 8, "HICCOUGH", ""))
            {
                MetaphAdd("P");
                handled = true;
            }

            //  special case: 'lough' alt spelling for Scots 'loch'
            if (StringAt(0, 5, "LOUGH", ""))
            {
                MetaphAdd("K");
                handled = true;
            }

            //  Hungarian
            if (StringAt(0, 6, "BALOGH", ""))
            {
                MetaphAddExactApprox("G", "", "K", "");
                handled = true;
            }

            //  "MacLaughlin"
            if (StringAt((m_current - 3), 8, "LAUGHLIN", "COUGHLAN", "LOUGHLIN", ""))
            {
                MetaphAdd("K", "F");
                handled = true;
            }
            else if ((StringAt((m_current - 3), 5, "GOUGH", "") || StringAt((m_current - 7), 9, "COLCLOUGH", "")))
            {
                MetaphAdd("", "F");
                handled = true;
            }

            if (handled)
            {
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_GH_To_F()
        {
            //  the cases covered here would fall under
            //  the GH_To_F rule below otherwise
            if (Encode_GH_Special_Cases())
            {
                return true;
            }
            else
            {
                // e.g., 'laugh', 'cough', 'rough', 'tough'
                if (((m_current > 2)
                            && ((CharAt((m_current - 1)) == 'U')
                            && (IsVowel((m_current - 2))
                            && (StringAt((m_current - 3), 1, "C", "G", "L", "R", "T", "N", "S", "")
                            && !StringAt((m_current - 4), 8, "BREUGHEL", "FLAUGHER", ""))))))
                {
                    MetaphAdd("F");
                    m_current += 2;
                    return true;
                }

            }

            return false;
        }

        private bool Encode_Silent_G()
        {
            //  e.g. "phlegm", "apothegm", "voigt"
            if (((((m_current + 1)
                        == m_last)
                        && (StringAt((m_current - 1), 3, "EGM", "IGM", "AGM", "") || StringAt(m_current, 2, "GT", "")))
                        || (StringAt(0, 5, "HUGES", "")
                        && (m_length == 5))))
            {
                m_current++;
                return true;
            }

            //  Vietnamese names e.g. "Nguyen" but not "Ng"
            if ((StringAt(0, 2, "NG", "")
                        && (m_current != m_last)))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_GN()
        {
            if (CharAt(m_current + 1) == 'N')
            {
                // 'align' 'sign', 'resign' but not 'resignation'
                // also 'impugn', 'impugnable', but not 'repugnant'
                if (((m_current > 1) && ((StringAt((m_current - 1), 1, "I", "U", "E", "")
                        || StringAt((m_current - 3), 9, "LORGNETTE", "") || StringAt((m_current - 2), 9, "LAGNIAPPE", "")
                        || StringAt((m_current - 2), 6, "COGNAC", "") || StringAt((m_current - 3), 7, "CHAGNON", "")
                        || StringAt((m_current - 5), 9, "COMPAGNIE", "") || StringAt((m_current - 4), 6, "BOLOGN", ""))
                        // Exceptions: following are cases where 'G' is pronounced
                        // in "assign" 'g' is silent, but not in "assignation"
                        && !(StringAt((m_current + 2), 5, "ATION", "") || StringAt((m_current + 2), 4, "ATOR", "")
                                || StringAt((m_current + 2), 3, "ATE", "ITY", "")
                                // exception to exceptions, not pronounced:
                                || (StringAt((m_current + 2), 2, "AN", "AC", "IA", "UM", "")
                                        && !(StringAt((m_current - 3), 8, "POIGNANT", "")
                                                || StringAt((m_current - 2), 6, "COGNAC", "")))
                                || StringAt(0, 7, "SPIGNER", "STEGNER", "")
                                || (StringAt(0, 5, "SIGNE", "") && (m_length == 5))
                                || StringAt((m_current - 2), 5, "LIGNI", "LIGNO", "REGNA", "DIGNI", "WEGNE", "TIGNE",
                                        "RIGNE", "REGNE", "TIGNO", "")
                                || StringAt((m_current - 2), 6, "SIGNAL", "SIGNIF", "SIGNAT", "")
                                || StringAt((m_current - 1), 5, "IGNIT", ""))
                        && !StringAt((m_current - 2), 6, "SIGNET", "LIGNEO", "")))
                        // not e.g. 'Cagney', 'magna'
                        || (((m_current + 2) == m_last) && StringAt(m_current, 3, "GNE", "GNA", "")
                                && !StringAt((m_current - 2), 5, "SIGNA", "MAGNA", "SIGNE", "")))
                {
                    MetaphAddExactApprox("N", "GN", "N", "KN");
                }
                else
                {
                    MetaphAddExactApprox("GN", "KN");
                }
                m_current += 2;
                return true;
            }
            return false;
        }

        private bool Encode_GL()
        {
            // 'tagliaro', 'puglia' BUT add K in alternative 
            //  since Americans sometimes do this
            if ((StringAt((m_current + 1), 3, "LIA", "LIO", "LIE", "") && IsVowel((m_current - 1))))
            {
                MetaphAddExactApprox("L", "GL", "L", "KL");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Initial_G_Soft()
        {
            if ((((StringAt((m_current + 1), 2, "EL", "EM", "EN", "EO", "ER", "ES", "IA", "IN", "IO", "IP", "IU", "YM", "YN", "YP", "YR", "EE", "") || StringAt((m_current + 1), 3, "IRA", "IRO", ""))
                        && !(StringAt((m_current + 1), 3, "ELD", "ELT", "ERT", "INZ", "ERH", "ITE", "ERD", "ERL", "ERN", "INT", "EES", "EEK", "ELB", "EER", "")
                        || (StringAt((m_current + 1), 4, "ERSH", "ERST", "INSB", "INGR", "EROW", "ERKE", "EREN", "")
                        || (StringAt((m_current + 1), 5, "ELLER", "ERDIE", "ERBER", "ESUND", "ESNER", "INGKO", "INKGO", "IPPER", "ESELL", "IPSON", "EEZER", "ERSON", "ELMAN", "")
                        || (StringAt((m_current + 1), 6, "ESTALT", "ESTAPO", "INGHAM", "ERRITY", "ERRISH", "ESSNER", "ENGLER", "")
                        || (StringAt((m_current + 1), 7, "YNAECOL", "YNECOLO", "ENTHNER", "ERAGHTY", "") || StringAt((m_current + 1), 8, "INGERICH", "EOGHEGAN", "")))))))
                        || (IsVowel((m_current + 1))
                        && (StringAt((m_current + 1), 3, "EE ", "EEW", "")
                        || ((StringAt((m_current + 1), 3, "IGI", "IRA", "IBE", "AOL", "IDE", "IGL", "")
                        && !StringAt((m_current + 1), 5, "IDEON", ""))
                        || (StringAt((m_current + 1), 4, "ILES", "INGI", "ISEL", "")
                        || ((StringAt((m_current + 1), 5, "INGER", "")
                        && !StringAt((m_current + 1), 8, "INGERICH", ""))
                        || (StringAt((m_current + 1), 5, "IBBER", "IBBET", "IBLET", "IBRAN", "IGOLO", "IRARD", "IGANT", "")
                        || (StringAt((m_current + 1), 6, "IRAFFE", "EEWHIZ", "") || StringAt((m_current + 1), 7, "ILLETTE", "IBRALTA", ""))))))))))
            {
                return true;
            }

            return false;
        }

        private bool Encode_Initial_G_Front_Vowel()
        {
            //  'g' followed by vowel at beginning
            if (((m_current == 0)
                        && Front_Vowel((m_current + 1))))
            {
                //  special case "gila" as in "gila monster"
                if ((StringAt((m_current + 1), 3, "ILA", "")
                            && (m_length == 4)))
                {
                    MetaphAdd("H");
                }
                else if (Initial_G_Soft())
                {
                    MetaphAddExactApprox("J", "G", "J", "K");
                }
                else
                {
                    //  only code alternate 'J' if front vowel
                    if (((m_inWord[(m_current + 1)] == 'E')
                                || (m_inWord[(m_current + 1)] == 'I')))
                    {
                        MetaphAddExactApprox("G", "J", "K", "J");
                    }
                    else
                    {
                        MetaphAddExactApprox("G", "K");
                    }

                }

                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_NGER()
        {
            if ((m_current > 1) && StringAt((m_current - 1), 4, "NGER", ""))
            {
                // default 'G' => J such as 'ranger', 'stranger', 'manger', 'messenger',
                // 'orangery', 'granger'
                // 'boulanger', 'challenger', 'danger', 'changer', 'harbinger', 'lounger',
                // 'ginger', 'passenger'
                // except for these the following
                if (!(RootOrInflections(m_inWord, "ANGER") || RootOrInflections(m_inWord, "LINGER")
                        || RootOrInflections(m_inWord, "MALINGER") || RootOrInflections(m_inWord, "FINGER")
                        || (StringAt((m_current - 3), 4, "HUNG", "FING", "BUNG", "WING", "RING", "DING", "ZENG", "ZING",
                                "JUNG", "LONG", "PING", "CONG", "MONG", "BANG", "GANG", "HANG", "LANG", "SANG", "SING",
                                "WANG", "ZANG", "")
                                // exceptions to above where 'G' => J
                                && !(StringAt((m_current - 6), 7, "BOULANG", "SLESING", "KISSING", "DERRING", "")
                                        || StringAt((m_current - 8), 9, "SCHLESING", "")
                                        || StringAt((m_current - 5), 6, "SALING", "BELANG", "")
                                        || StringAt((m_current - 6), 7, "BARRING", "")
                                        || StringAt((m_current - 6), 9, "PHALANGER", "")
                                        || StringAt((m_current - 4), 5, "CHANG", "")))
                        || StringAt((m_current - 4), 5, "STING", "YOUNG", "") || StringAt((m_current - 5), 6, "STRONG", "")
                        || StringAt(0, 3, "UNG", "ENG", "ING", "") || StringAt(m_current, 6, "GERICH", "")
                        || StringAt(0, 6, "SENGER", "")
                        || StringAt((m_current - 3), 6, "WENGER", "MUNGER", "SONGER", "KINGER", "")
                        || StringAt((m_current - 4), 7, "FLINGER", "SLINGER", "STANGER", "STENGER", "KLINGER", "CLINGER",
                                "")
                        || StringAt((m_current - 5), 8, "SPRINGER", "SPRENGER", "")
                        || StringAt((m_current - 3), 7, "LINGERF", "")
                        || StringAt((m_current - 2), 7, "ANGERLY", "ANGERBO", "INGERSO", "")))
                {
                    MetaphAddExactApprox("J", "G", "J", "K");
                }
                else
                {
                    MetaphAddExactApprox("G", "J", "K", "J");
                }

                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_GER()
        {
            if ((m_current > 0) && StringAt((m_current + 1), 2, "ER", ""))
            {
                // Exceptions to 'GE' where 'G' => K
                // e.g. "JAGER", "TIGER", "LIGER", "LAGER", "LUGER", "AUGER", "EAGER", "HAGER",
                // "SAGER"
                if ((((m_current == 2) && IsVowel(m_current - 1) && !IsVowel(m_current - 2)
                        && !(StringAt((m_current - 2), 5, "PAGER", "WAGER", "NIGER", "ROGER", "LEGER", "CAGER", ""))
                        || StringAt((m_current - 2), 5, "AUGER", "EAGER", "INGER", "YAGER", ""))
                        || StringAt((m_current - 3), 6, "SEEGER", "JAEGER", "GEIGER", "KRUGER", "SAUGER", "BURGER",
                                "MEAGER", "MARGER", "RIEGER", "YAEGER", "STEGER", "PRAGER", "SWIGER", "YERGER", "TORGER",
                                "FERGER", "HILGER", "ZEIGER", "YARGER", "COWGER", "CREGER", "KROGER", "KREGER", "GRAGER",
                                "STIGER", "BERGER", "")
                        // 'berger' but not 'Bergerac'
                        || (StringAt((m_current - 3), 6, "BERGER", "") && ((m_current + 2) == m_last))
                        || StringAt((m_current - 4), 7, "KREIGER", "KRUEGER", "METZGER", "KRIEGER", "KROEGER", "STEIGER",
                                "DRAEGER", "BUERGER", "BOERGER", "FIBIGER", "")
                        // e.g. 'harshbarger', 'winebarger'
                        || (StringAt((m_current - 3), 6, "BARGER", "") && (m_current > 4))
                        // e.g. 'weisgerber'
                        || (StringAt(m_current, 6, "GERBER", "") && (m_current > 0))
                        || StringAt((m_current - 5), 8, "SCHWAGER", "LYBARGER", "SPRENGER", "GALLAGER", "WILLIGER", "")
                        || StringAt(0, 6, "HARGER", "") || (StringAt(0, 4, "AGER", "EGER", "") && (m_length == 4))
                        || StringAt((m_current - 1), 6, "YGERNE", "") || StringAt((m_current - 6), 9, "SCHWEIGER", ""))
                        && !(StringAt((m_current - 5), 10, "BELLIGEREN", "") || StringAt(0, 7, "MARGERY", "")
                                || StringAt((m_current - 3), 8, "BERGERAC", "")))
                {
                    if (SlavoGermanic())
                    {
                        MetaphAddExactApprox("G", "K");
                    }
                    else
                    {
                        MetaphAddExactApprox("G", "J", "K", "J");
                    }
                }
                else
                {
                    MetaphAddExactApprox("J", "G", "J", "K");
                }

                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_GEL()
        {
            //  more likely to be "-GEL-" => JL
            if ((StringAt((m_current + 1), 2, "EL", "")
                        && (m_current > 0)))
            {
                //  except for
                //  "BAGEL", "HEGEL", "HUGEL", "KUGEL", "NAGEL", "VOGEL", "FOGEL", "PAGEL"
                if ((((m_length == 5)
                            && (IsVowel((m_current - 1))
                            && (!IsVowel((m_current - 2))
                            && !StringAt((m_current - 2), 5, "NIGEL", "RIGEL", ""))))
                            || (StringAt((m_current - 2), 5, "ENGEL", "HEGEL", "NAGEL", "VOGEL", "")
                            || (StringAt((m_current - 3), 6, "MANGEL", "WEIGEL", "FLUGEL", "RANGEL", "HAUGEN", "RIEGEL", "VOEGEL", "")
                            || (StringAt((m_current - 4), 7, "SPEIGEL", "STEIGEL", "WRANGEL", "SPIEGEL", "") || StringAt((m_current - 4), 8, "DANEGELD", ""))))))
                {
                    if (SlavoGermanic())
                    {
                        MetaphAddExactApprox("G", "K");
                    }
                    else
                    {
                        MetaphAddExactApprox("G", "J", "K", "J");
                    }

                }
                else
                {
                    MetaphAddExactApprox("J", "G", "J", "K");
                }

                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_Non_Initial_G_Front_Vowel()
        {
            //  -gy-, gi-, ge-
            if (StringAt((m_current + 1), 1, "E", "I", "Y", ""))
            {
                //  '-ge' at end
                //  almost always 'j 'sound
                if ((StringAt(m_current, 2, "GE", "")
                            && (m_current
                            == (m_last - 1))))
                {
                    if (Hard_GE_At_End())
                    {
                        if (SlavoGermanic())
                        {
                            MetaphAddExactApprox("G", "K");
                        }
                        else
                        {
                            MetaphAddExactApprox("G", "J", "K", "J");
                        }

                    }
                    else
                    {
                        MetaphAdd("J");
                    }

                }
                else if (Internal_Hard_G())
                {
                    //  don't encode KG or KK if e.g. "McGill"
                    if ((!((m_current == 2)
                                && StringAt(0, 2, "MC", ""))
                                || ((m_current == 3)
                                && StringAt(0, 3, "MAC", ""))))
                    {
                        if (SlavoGermanic())
                        {
                            MetaphAddExactApprox("G", "K");
                        }
                        else
                        {
                            MetaphAddExactApprox("G", "J", "K", "J");
                        }

                    }

                }
                else
                {
                    MetaphAddExactApprox("J", "G", "J", "K");
                }

                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Hard_GE_At_End()
        {
            if ((StringAt(0, 6, "RENEGE", "STONGE", "STANGE", "PRANGE", "KRESGE", "")
                        || (StringAt(0, 5, "BYRGE", "BIRGE", "BERGE", "HAUGE", "")
                        || (StringAt(0, 4, "HAGE", "")
                        || (StringAt(0, 5, "LANGE", "SYNGE", "BENGE", "RUNGE", "HELGE", "") || StringAt(0, 4, "INGE", "LAGE", ""))))))
            {
                return true;
            }

            return false;
        }

        private bool Internal_Hard_G()
        {
            //  if not "-GE" at end
            if ((!(((m_current + 1)
                        == m_last)
                        && (CharAt((m_current + 1)) == 'E'))
                        && (Internal_Hard_NG()
                        || (Internal_Hard_GEN_GIN_GET_GIT()
                        || (Internal_Hard_G_Open_Syllable() || Internal_Hard_G_Other())))))
            {
                return true;
            }

            return false;
        }

        private bool Internal_Hard_G_Other()
        {
            if (((StringAt(m_current, 4, "GETH", "GEAR", "GEIS", "GIRL", "GIVI", "GIVE", "GIFT", "GIRD", "GIRT", "GILV", "GILD", "GELD", "")
                        && !StringAt((m_current - 3), 6, "GINGIV", ""))
                        || ((StringAt((m_current + 1), 3, "ISH", "")
                        && ((m_current > 0)
                        && !StringAt(0, 4, "LARG", "")))
                        || ((StringAt((m_current - 2), 5, "MAGED", "MEGID", "")
                        && !((m_current + 2)
                        == m_last))
                        || (StringAt(m_current, 3, "GEZ", "")
                        || (StringAt(0, 4, "WEGE", "HAGE", "")
                        || ((StringAt((m_current - 2), 6, "ONGEST", "UNGEST", "")
                        && (((m_current + 3)
                        == m_last)
                        && !StringAt((m_current - 3), 7, "CONGEST", "")))
                        || (StringAt(0, 5, "VOEGE", "BERGE", "HELGE", "")
                        || ((StringAt(0, 4, "ENGE", "BOGY", "")
                        && (m_length == 4))
                        || (StringAt(m_current, 6, "GIBBON", "")
                        || (StringAt(0, 10, "CORREGIDOR", "")
                        || (StringAt(0, 8, "INGEBORG", "")
                        || (StringAt(m_current, 4, "GILL", "")
                        && ((((m_current + 3)
                        == m_last)
                        || ((m_current + 4)
                        == m_last))
                        && !StringAt(0, 8, "STURGILL", "")))))))))))))))
            {
                return true;
            }

            return false;
        }

        private bool Internal_Hard_G_Open_Syllable()
        {
            if ((StringAt((m_current + 1), 3, "EYE", "")
                        || (StringAt((m_current - 2), 4, "FOGY", "POGY", "YOGI", "")
                        || (StringAt((m_current - 2), 5, "MAGEE", "MCGEE", "HAGIO", "")
                        || (StringAt((m_current - 1), 4, "RGEY", "OGEY", "")
                        || (StringAt((m_current - 3), 5, "HOAGY", "STOGY", "PORGY", "")
                        || (StringAt((m_current - 5), 8, "CARNEGIE", "")
                        || (StringAt((m_current - 1), 4, "OGEY", "OGIE", "")
                        && ((m_current + 2)
                        == m_last)))))))))
            {
                return true;
            }

            return false;
        }

        private bool Internal_Hard_GEN_GIN_GET_GIT()
        {
            if (((StringAt((m_current - 3), 6, "FORGET", "TARGET", "MARGIT", "MARGET", "TURGEN", "BERGEN", "MORGEN", "JORGEN", "HAUGEN", "JERGEN", "JURGEN", "LINGEN", "BORGEN", "LANGEN", "KLAGEN", "STIGER", "BERGER", "")
                        && (!StringAt(m_current, 7, "GENETIC", "GENESIS", "")
                        && !StringAt((m_current - 4), 8, "PLANGENT", "")))
                        || ((StringAt((m_current - 3), 6, "BERGIN", "FEAGIN", "DURGIN", "")
                        && ((m_current + 2)
                        == m_last))
                        || ((StringAt((m_current - 2), 5, "ENGEN", "")
                        && !StringAt((m_current + 3), 3, "DER", "ETI", "ESI", ""))
                        || (StringAt((m_current - 4), 7, "JUERGEN", "")
                        || (StringAt(0, 5, "NAGIN", "MAGIN", "HAGIN", "")
                        || ((StringAt(0, 5, "ENGIN", "DEGEN", "LAGEN", "MAGEN", "NAGIN", "")
                        && (m_length == 5))
                        || (StringAt((m_current - 2), 5, "BEGET", "BEGIN", "HAGEN", "FAGIN", "BOGEN", "WIGIN", "NTGEN", "EIGEN", "WEGEN", "WAGEN", "")
                        && !StringAt((m_current - 5), 8, "OSPHAGEN", "")))))))))
            {
                return true;
            }

            return false;
        }

        private bool Internal_Hard_NG()
        {
            if ((StringAt((m_current - 3), 4, "DANG", "FANG", "SING", "")
                    // exception to exception
                    && !StringAt((m_current - 5), 8, "DISINGEN", ""))
                    || StringAt(0, 5, "INGEB", "ENGEB", "")
                    || (StringAt((m_current - 3), 4, "RING", "WING", "HANG", "LONG", "")
                            && !(StringAt((m_current - 4), 5, "CRING", "FRING", "ORANG", "TWING", "CHANG", "PHANG", "")
                                    || StringAt((m_current - 5), 6, "SYRING", "")
                                    || StringAt((m_current - 3), 7, "RINGENC", "RINGENT", "LONGITU", "LONGEVI", "")
                                    // e.g. 'longino', 'mastrangelo'
                                    || (StringAt(m_current, 4, "GELO", "GINO", "") && ((m_current + 3) == m_last))))
                    || (StringAt((m_current - 1), 3, "NGY", "")
                            // exceptions to exception
                            && !(StringAt((m_current - 3), 5, "RANGY", "MANGY", "MINGY", "")
                                    || StringAt((m_current - 4), 6, "SPONGY", "STINGY", ""))))
            {
                return true;
            }

            return false;
        }

        private bool Encode_GA_To_J()
        {
            // 'margary', 'margarine'
            if ((StringAt((m_current - 3), 7, "MARGARY", "MARGARI", "")
                    // but not in Spanish forms such as "margatita"
                    && !StringAt((m_current - 3), 8, "MARGARIT", "")) || StringAt(0, 4, "GAOL", "")
                    || StringAt((m_current - 2), 5, "ALGAE", ""))
            {
                MetaphAddExactApprox("J", "G", "J", "K");
                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private void Encode_H()
        {
            if ((Encode_Initial_Silent_H()
                        || (Encode_Initial_HS()
                        || (Encode_Initial_HU_HW() || Encode_Non_Initial_Silent_H()))))
            {
                return;
            }

            // only keep if first & before vowel or btw. 2 vowels
            if (!Encode_H_Pronounced())
            {
                // also takes care of 'HH'
                m_current++;
            }

        }

        private bool Encode_Initial_Silent_H()
        {
            // 'hour', 'herb', 'heir', 'honor'
            if ((StringAt((m_current + 1), 3, "OUR", "ERB", "EIR", "")
                        || (StringAt((m_current + 1), 4, "ONOR", "") || StringAt((m_current + 1), 5, "ONOUR", "ONEST", ""))))
            {
                //  British pronounce H in this word
                //  Americans give it 'H' for the name,
                //  no 'H' for the plant
                if (((m_current == 0)
                            && StringAt(m_current, 4, "HERB", "")))
                {
                    if (m_encodeVowels)
                    {
                        MetaphAdd("HA", "A");
                    }
                    else
                    {
                        MetaphAdd("H", "A");
                    }

                }
                else if (((m_current == 0)
                            || m_encodeVowels))
                {
                    MetaphAdd("A");
                }

                m_current++;
                //  don't encode vowels twice
                m_current = SkipVowels(m_current);
                return true;
            }

            return false;
        }

        private bool Encode_Initial_HS()
        {
            //  old Chinese pinyin transliteration
            //  e.g., 'HSIAO'
            if (((m_current == 0)
                        && StringAt(0, 2, "HS", "")))
            {
                MetaphAdd("X");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_Initial_HU_HW()
        {
            //  Spanish spellings and Chinese pinyin transliteration
            if (StringAt(0, 3, "HUA", "HUE", "HWA", ""))
            {
                if (!StringAt(m_current, 4, "HUEY", ""))
                {
                    MetaphAdd("A");
                    if (!m_encodeVowels)
                    {
                        m_current += 3;
                    }
                    else
                    {
                        m_current++;
                        //  don't encode vowels twice
                        while ((IsVowel(m_current)
                                    || (CharAt(m_current) == 'W')))
                        {
                            m_current++;
                        }

                    }

                    return true;
                }

            }

            return false;
        }

        private bool Encode_Non_Initial_Silent_H()
        {
            // exceptions - 'h' not pronounced
            //  "PROHIB" BUT NOT "PROHIBIT"
            if ((StringAt((m_current - 2), 5, "NIHIL", "VEHEM", "LOHEN", "NEHEM", "MAHON", "MAHAN", "COHEN", "GAHAN", "")
                        || (StringAt((m_current - 3), 6, "GRAHAM", "PROHIB", "FRAHER", "TOOHEY", "TOUHEY", "")
                        || (StringAt((m_current - 3), 5, "TOUHY", "") || StringAt(0, 9, "CHIHUAHUA", "")))))
            {
                if (!m_encodeVowels)
                {
                    m_current += 2;
                }
                else
                {
                    m_current++;
                    //  don't encode vowels twice
                    m_current = SkipVowels(m_current);
                }

                return true;
            }

            return false;
        }

        private bool Encode_H_Pronounced()
        {
            if (((((m_current == 0)
                        || (IsVowel((m_current - 1))
                        || ((m_current > 0)
                        && (CharAt((m_current - 1)) == 'W'))))
                        && IsVowel((m_current + 1)))
                        || ((CharAt((m_current + 1)) == 'H')
                        && IsVowel((m_current + 2)))))
            {
                MetaphAdd("H");
                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private void Encode_J()
        {
            if ((Encode_Spanish_J() || Encode_Spanish_OJ_UJ()))
            {
                return;
            }

            Encode_Other_J();
        }

        private bool Encode_Spanish_J()
        {
            // obvious Spanish, e.g. "jose", "San jacinto"
            if (((StringAt((m_current + 1), 3, "UAN", "ACI", "ALI", "EFE", "ICA", "IME", "OAQ", "UAR", "")
                        && !StringAt(m_current, 8, "JIMERSON", "JIMERSEN", ""))
                        || ((StringAt((m_current + 1), 3, "OSE", "")
                        && ((m_current + 3)
                        == m_last))
                        || (StringAt((m_current + 1), 4, "EREZ", "UNTA", "AIME", "AVIE", "AVIA", "")
                        || (StringAt((m_current + 1), 6, "IMINEZ", "ARAMIL", "")
                        || ((((m_current + 2)
                        == m_last)
                        && StringAt((m_current - 2), 5, "MEJIA", ""))
                        || (StringAt((m_current - 2), 5, "TEJED", "TEJAD", "LUJAN", "FAJAR", "BEJAR", "BOJOR", "CAJIG", "DEJAS", "DUJAR", "DUJAN", "MIJAR", "MEJOR", "NAJAR", "NOJOS", "RAJED", "RIJAL", "REJON", "TEJAN", "UIJAN", "")
                        || (StringAt((m_current - 3), 8, "ALEJANDR", "GUAJARDO", "TRUJILLO", "")
                        || ((StringAt((m_current - 2), 5, "RAJAS", "")
                        && (m_current > 2))
                        || ((StringAt((m_current - 2), 5, "MEJIA", "")
                        && !StringAt((m_current - 2), 6, "MEJIAN", ""))
                        || (StringAt((m_current - 1), 5, "OJEDA", "")
                        || (StringAt((m_current - 3), 5, "LEIJA", "MINJA", "")
                        || (StringAt((m_current - 3), 6, "VIAJES", "GRAJAL", "")
                        || (StringAt(m_current, 8, "JAUREGUI", "")
                        || (StringAt((m_current - 4), 8, "HINOJOSA", "")
                        || (StringAt(0, 4, "SAN ", "")
                        || (((m_current + 1)
                        == m_last)
                        && ((CharAt((m_current + 1)) == 'O')
                        && !(StringAt(0, 4, "TOJO", "")
                        || (StringAt(0, 5, "BANJO", "") || StringAt(0, 6, "MARYJO", "")))))))))))))))))))))
            {
                //  Americans pronounce "Juan" as 'wan'
                //  and "marijuana" and "Tijuana" also
                //  do not get the 'H' as in Spanish, so
                //  just treat it like a vowel in these cases
                if (!(StringAt(m_current, 4, "JUAN", "") || StringAt(m_current, 4, "JOAQ", "")))
                {
                    MetaphAdd("H");
                }
                else if ((m_current == 0))
                {
                    MetaphAdd("A");
                }

                AdvanceCounter(2, 1);
                return true;
            }

            //  Jorge gets 2nd HARHA. also JULIO, JESUS
            if ((StringAt((m_current + 1), 4, "ORGE", "ULIO", "ESUS", "")
                        && !StringAt(0, 6, "JORGEN", "")))
            {
                //  get both consonants for "Jorge"
                if ((((m_current + 4)
                            == m_last)
                            && StringAt((m_current + 1), 4, "ORGE", "")))
                {
                    if (m_encodeVowels)
                    {
                        MetaphAdd("JARJ", "HARHA");
                    }
                    else
                    {
                        MetaphAdd("JRJ", "HRH");
                    }

                    AdvanceCounter(5, 5);
                    return true;
                }

                MetaphAdd("J", "H");
                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_German_J()
        {
            if ((StringAt((m_current + 1), 2, "AH", "")
                        || ((StringAt((m_current + 1), 5, "OHANN", "")
                        && ((m_current + 5)
                        == m_last))
                        || ((StringAt((m_current + 1), 3, "UNG", "")
                        && !StringAt((m_current + 1), 4, "UNGL", ""))
                        || StringAt((m_current + 1), 3, "UGO", "")))))
            {
                MetaphAdd("A");
                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_Spanish_OJ_UJ()
        {
            if (StringAt((m_current + 1), 5, "OJOBA", "UJUY ", ""))
            {
                if (m_encodeVowels)
                {
                    MetaphAdd("HAH");
                }
                else
                {
                    MetaphAdd("HH");
                }

                AdvanceCounter(4, 3);
                return true;
            }

            return false;
        }

        private bool Encode_J_To_J()
        {
            if (IsVowel((m_current + 1)))
            {
                if (((m_current == 0)
                            && Names_Beginning_With_J_That_Get_Alt_Y()))
                {
                    //  'Y' is a vowel so encode
                    //  is as 'A'
                    if (m_encodeVowels)
                    {
                        MetaphAdd("JA", "A");
                    }
                    else
                    {
                        MetaphAdd("J", "A");
                    }

                }
                else if (m_encodeVowels)
                {
                    MetaphAdd("JA");
                }
                else
                {
                    MetaphAdd("J");
                }

                m_current++;
                m_current = SkipVowels(m_current);
                return false;
            }
            else
            {
                MetaphAdd("J");
                m_current++;
                return true;
            }

            //         return false;
        }

        private bool Encode_Spanish_J_2()
        {
            //  Spanish forms e.g. "brujo", "badajoz"
            if (((((m_current - 2)
                        == 0)
                        && StringAt((m_current - 2), 4, "BOJA", "BAJA", "BEJA", "BOJO", "MOJA", "MOJI", "MEJI", ""))
                        || ((((m_current - 3)
                        == 0)
                        && StringAt((m_current - 3), 5, "FRIJO", "BRUJO", "BRUJA", "GRAJE", "GRIJA", "LEIJA", "QUIJA", ""))
                        || ((((m_current + 3)
                        == m_last)
                        && StringAt((m_current - 1), 5, "AJARA", ""))
                        || ((((m_current + 2)
                        == m_last)
                        && StringAt((m_current - 1), 4, "AJOS", "EJOS", "OJAS", "OJOS", "UJON", "AJOZ", "AJAL", "UJAR", "EJON", "EJAN", ""))
                        || (((m_current + 1)
                        == m_last)
                        && (StringAt((m_current - 1), 3, "OJA", "EJA", "")
                        && !StringAt(0, 4, "DEJA", ""))))))))
            {
                MetaphAdd("H");
                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_J_As_Vowel()
        {
            if (StringAt(m_current, 5, "JEWSK", ""))
            {
                MetaphAdd("J", "");
                return true;
            }

            // e.g. "stijl", "sejm" - dutch, Scandinavian, and eastern European spellings
            if ((StringAt((m_current + 1), 1, "L", "T", "K", "S", "N", "M", "")
                    // except words from Hindi and Arabic
                    && !StringAt((m_current + 2), 1, "A", "")) || StringAt(0, 9, "HALLELUJA", "LJUBLJANA", "")
                    || StringAt(0, 4, "LJUB", "BJOR", "") || StringAt(0, 5, "HAJEK", "") || StringAt(0, 3, "WOJ", "")
                    // e.g. 'fjord'
                    || StringAt(0, 2, "FJ", "")
                    // e.g. 'rekjavik', 'blagojevic'
                    || StringAt(m_current, 5, "JAVIK", "JEVIC", "")
                    || (((m_current + 1) == m_last) && StringAt(0, 5, "SONJA", "TANJA", "TONJA", "")))

            {
                return true;
            }
            return false;
        }

        private void Encode_Other_J()
        {
            if ((m_current == 0))
            {
                if (Encode_German_J())
                {
                    return;
                }
                else if (Encode_J_To_J())
                {
                    return;
                }

            }
            else
            {
                if (Encode_Spanish_J_2())
                {
                    return;
                }
                else if (!Encode_J_As_Vowel())
                {
                    MetaphAdd("J");
                }

                // it could happen! e.g. "hajj"
                //  eat redundant 'J'
                if ((CharAt((m_current + 1)) == 'J'))
                {
                    m_current += 2;
                }
                else
                {
                    m_current++;
                }

            }

        }

        private void Encode_K()
        {
            if (!Encode_Silent_K())
            {
                MetaphAdd("K");
                //  eat redundant 'K's and 'Q's
                if (((CharAt((m_current + 1)) == 'K')
                            || (CharAt((m_current + 1)) == 'Q')))
                {
                    m_current += 2;
                }
                else
                {
                    m_current++;
                }

            }

        }

        private bool Encode_Silent_K()
        {
            // skip this except for special cases
            if ((m_current == 0) && StringAt(m_current, 2, "KN", ""))
            {
                if (!(StringAt((m_current + 2), 5, "ESSET", "IEVEL", "") || StringAt((m_current + 2), 3, "ISH", "")))
                {
                    m_current += 1;
                    return true;
                }
            }

            // e.g. "know", "knit", "knob"
            if ((StringAt((m_current + 1), 3, "NOW", "NIT", "NOT", "NOB", "")
                    // exception, "slipknot" => SLPNT but "banknote" => PNKNT
                    && !StringAt(0, 8, "BANKNOTE", "")) || StringAt((m_current + 1), 4, "NOCK", "NUCK", "NIFE", "NACK", "")
                    || StringAt((m_current + 1), 5, "NIGHT", ""))
            {
                // N already encoded before
                // e.g. "penknife"
                if ((m_current > 0) && CharAt(m_current - 1) == 'N')
                {
                    m_current += 2;
                }
                else
                {
                    m_current++;
                }

                return true;
            }

            return false;
        }


        private void Encode_L()
        {
            //  logic below needs to know this
            //  after 'm_current' variable changed 
            int save_current = m_current;
            Interpolate_Vowel_When_Cons_L_At_End();
            if ((Encode_LELY_To_L()
                        || (Encode_COLONEL()
                        || (Encode_French_AULT()
                        || (Encode_French_EUIL()
                        || (Encode_French_OULX()
                        || (Encode_Silent_L_In_LM()
                        || (Encode_Silent_L_In_LK_LV() || Encode_Silent_L_In_OULD()))))))))
            {
                return;
            }

            if (Encode_LL_As_Vowel_Cases())
            {
                return;
            }

            Encode_LE_Cases(save_current);
        }

        private void Interpolate_Vowel_When_Cons_L_At_End()
        {
            if ((m_encodeVowels == true))
            {
                //  e.g. "ertl", "vogl"
                if (((m_current == m_last)
                            && StringAt((m_current - 1), 1, "D", "G", "T", "")))
                {
                    MetaphAdd("A");
                }

            }

        }

        private bool Encode_LELY_To_L()
        {
            //  e.g. "agilely", "docilely"
            if ((StringAt((m_current - 1), 5, "ILELY", "")
                        && ((m_current + 3)
                        == m_last)))
            {
                MetaphAdd("L");
                m_current += 3;
                return true;
            }

            return false;
        }

        private bool Encode_COLONEL()
        {
            if (StringAt((m_current - 2), 7, "COLONEL", ""))
            {
                MetaphAdd("R");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_French_AULT()
        {
            //  e.g. "Renault" and "Foucault", well known to Americans, but not "fault"
            if (((m_current > 3)
                        && ((StringAt((m_current - 3), 5, "RAULT", "NAULT", "BAULT", "SAULT", "GAULT", "CAULT", "") || StringAt((m_current - 4), 6, "REAULT", "RIAULT", "NEAULT", "BEAULT", ""))
                        && !(RootOrInflections(m_inWord, "ASSAULT")
                        || (StringAt((m_current - 8), 10, "SOMERSAULT", "") || StringAt((m_current - 9), 11, "SUMMERSAULT", ""))))))
            {
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_French_EUIL()
        {
            //  e.g. "auteuil"
            if ((StringAt((m_current - 3), 4, "EUIL", "")
                        && (m_current == m_last)))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_French_OULX()
        {
            //  e.g. "proulx"
            if ((StringAt((m_current - 2), 4, "OULX", "")
                        && ((m_current + 1)
                        == m_last)))
            {
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_Silent_L_In_LM()
        {
            if (StringAt(m_current, 2, "LM", "LN", ""))
            {
                //  e.g. "Lincoln", "Holmes", "psalm", "salmon"
                if (((StringAt((m_current - 2), 4, "COLN", "CALM", "BALM", "MALM", "PALM", "")
                            || ((StringAt((m_current - 1), 3, "OLM", "")
                            && ((m_current + 1)
                            == m_last))
                            || (StringAt((m_current - 3), 5, "PSALM", "QUALM", "")
                            || (StringAt((m_current - 2), 6, "SALMON", "HOLMES", "")
                            || (StringAt((m_current - 1), 6, "ALMOND", "")
                            || ((m_current == 1)
                            && StringAt((m_current - 1), 4, "ALMS", "")))))))
                            && (!StringAt((m_current + 2), 1, "A", "")
                            && (!StringAt((m_current - 2), 5, "BALMO", "")
                            && (!StringAt((m_current - 2), 6, "PALMER", "PALMOR", "BALMER", "")
                            && !StringAt((m_current - 3), 5, "THALM", ""))))))
                {
                    m_current++;
                    return true;
                }
                else
                {
                    MetaphAdd("L");
                    m_current++;
                    return true;
                }

            }

            return false;
        }

        private bool Encode_Silent_L_In_LK_LV()
        {
            if (((StringAt((m_current - 2), 4, "WALK", "YOLK", "FOLK", "HALF", "TALK", "CALF", "BALK", "CALK", "")
                        || ((StringAt((m_current - 2), 4, "POLK", "")
                        && !StringAt((m_current - 2), 5, "POLKA", "WALKO", ""))
                        || ((StringAt((m_current - 2), 4, "HALV", "")
                        && !StringAt((m_current - 2), 5, "HALVA", "HALVO", ""))
                        || ((StringAt((m_current - 3), 5, "CAULK", "CHALK", "BAULK", "FAULK", "")
                        && !StringAt((m_current - 4), 6, "SCHALK", ""))
                        || ((StringAt((m_current - 2), 5, "SALVE", "CALVE", "") || StringAt((m_current - 2), 6, "SOLDER", ""))
                        && !StringAt((m_current - 2), 6, "SALVER", "CALVER", ""))))))
                        && (!StringAt((m_current - 5), 9, "GONSALVES", "GONCALVES", "")
                        && (!StringAt((m_current - 2), 6, "BALKAN", "TALKAL", "")
                        && !StringAt((m_current - 3), 5, "PAULK", "CHALF", "")))))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_Silent_L_In_OULD()
        {
            // 'would', 'could'
            if ((StringAt((m_current - 3), 5, "WOULD", "COULD", "")
                        || (StringAt((m_current - 4), 6, "SHOULD", "")
                        && !StringAt((m_current - 4), 8, "SHOULDER", ""))))
            {
                MetaphAddExactApprox("D", "T");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_LL_As_Vowel_Special_Cases()
        {
            if (StringAt((m_current - 5), 8, "TORTILLA", "") || StringAt((m_current - 8), 11, "RATATOUILLE", "")
                    // e.g. 'Guillermo', "Veillard"
                    || (StringAt(0, 5, "GUILL", "VEILL", "GAILL", "")
                            // 'guillotine' usually has '-ll-' pronounced as 'L' in English
                            && !(StringAt((m_current - 3), 7, "GUILLOT", "GUILLOR", "GUILLEN", "")
                                    || (StringAt(0, 5, "GUILL", "") && (m_length == 5))))
                    // e.g. "brouillard", "gremillion"
                    || StringAt(0, 7, "BROUILL", "GREMILL", "")
                    || StringAt(0, 6, "ROBILL", "")
                    // e.g. 'Mireille'
                    || (StringAt((m_current - 2), 5, "EILLE", "") && ((m_current + 2) == m_last)
                            // exception "reveille" usually pronounced as 're-vil-lee'
                            && !StringAt((m_current - 5), 8, "REVEILLE", "")))
            {
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_LL_As_Vowel()
        {
            // Spanish e.g. "cabrillo", "Gallegos" but also "gorilla", "ballerina" -
            //  give both pronunciations since an American might pronounce "cabrillo"
            //  in the Spanish or the American fashion.
            if (((((m_current + 3)
                        == m_length)
                        && StringAt((m_current - 1), 4, "ILLO", "ILLA", "ALLE", ""))
                        || ((((StringAt((m_last - 1), 2, "AS", "OS", "")
                        || (StringAt(m_last, 2, "AS", "OS", "") || StringAt(m_last, 1, "A", "O", "")))
                        && StringAt((m_current - 1), 2, "AL", "IL", ""))
                        && !StringAt((m_current - 1), 4, "ALLA", ""))
                        || (StringAt(0, 5, "VILLE", "VILLA", "")
                        || (StringAt(0, 8, "GALLARDO", "VALLADAR", "MAGALLAN", "CAVALLAR", "BALLASTE", "") || StringAt(0, 3, "LLA", ""))))))
            {
                MetaphAdd("L", "");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_LL_As_Vowel_Cases()
        {
            if ((CharAt((m_current + 1)) == 'L'))
            {
                if (Encode_LL_As_Vowel_Special_Cases())
                {
                    return true;
                }
                else if (Encode_LL_As_Vowel())
                {
                    return true;
                }

                m_current += 2;
            }
            else
            {
                m_current++;
            }

            return false;
        }

        private bool Encode_Vowel_LE_Transposition(int save_current)
        {
            // transposition of vowel sound and L occurs in many words,
            // e.g. "bristle", "dazzle", "goggle" => KAKAL
            if (m_encodeVowels && (save_current > 1) && !IsVowel(save_current - 1) && (CharAt(save_current + 1) == 'E')
                    && (CharAt(save_current - 1) != 'L') && (CharAt(save_current - 1) != 'R')
                    // lots of exceptions to this:
                    && !IsVowel(save_current + 2) && !StringAt(0, 7, "ECCLESI", "COMPLEC", "COMPLEJ", "ROBLEDO", "")
                    && !StringAt(0, 5, "MCCLE", "MCLEL", "") && !StringAt(0, 6, "EMBLEM", "KADLEC", "")
                    && !(((save_current + 2) == m_last) && StringAt(save_current, 3, "LET", ""))
                    && !StringAt(save_current, 7, "LETTING", "")
                    && !StringAt(save_current, 6, "LETELY", "LETTER", "LETION", "LETIAN", "LETING", "LETORY", "")
                    && !StringAt(save_current, 5, "LETUS", "LETIV", "")
                    && !StringAt(save_current, 4, "LESS", "LESQ", "LECT", "LEDG", "LETE", "LETH", "LETS", "LETT", "")
                    && !StringAt(save_current, 3, "LEG", "LER", "LEX", "")
                    // e.g. "complement" !=> KAMPALMENT
                    && !(StringAt(save_current, 6, "LEMENT", "")
                            && !(StringAt((m_current - 5), 6, "BATTLE", "TANGLE", "PUZZLE", "RABBLE", "BABBLE", "")
                                    || StringAt((m_current - 4), 5, "TABLE", "")))
                    && !(((save_current + 2) == m_last) && StringAt((save_current - 2), 5, "OCLES", "ACLES", "AKLES", ""))
                    && !StringAt((save_current - 3), 5, "LISLE", "AISLE", "") && !StringAt(0, 4, "ISLE", "")
                    && !StringAt(0, 6, "ROBLES", "") && !StringAt((save_current - 4), 7, "PROBLEM", "RESPLEN", "")
                    && !StringAt((save_current - 3), 6, "REPLEN", "") && !StringAt((save_current - 2), 4, "SPLE", "")
                    && (CharAt(save_current - 1) != 'H') && (CharAt(save_current - 1) != 'W'))
            {
                MetaphAdd("AL");
                flag_AL_inversion = true;

                // eat redundant 'L'
                if (CharAt(save_current + 2) == 'L')
                {
                    m_current = save_current + 3;
                }
                return true;
            }

            return false;
        }


        private bool Encode_Vowel_Preserve_Vowel_After_L(int save_current)
        {
            //  an example of where the vowel would NOT need to be preserved
            //  would be, say, "hustled", where there is no vowel pronounced
            //  between the 'l' and the 'd'
            if ((m_encodeVowels
                        && (!IsVowel((save_current - 1))
                        && ((CharAt((save_current + 1)) == 'E')
                        && ((save_current > 1)
                        && (((save_current + 1)
                        != m_last)
                        && (!(StringAt((save_current + 1), 2, "ES", "ED", "")
                        && ((save_current + 2)
                        == m_last))
                        && !StringAt((save_current - 1), 5, "RLEST", ""))))))))
            {
                MetaphAdd("LA");
                m_current = SkipVowels(m_current);
                return true;
            }

            return false;
        }

        private void Encode_LE_Cases(int save_current)
        {
            if (Encode_Vowel_LE_Transposition(save_current))
            {
                return;
            }
            else if (Encode_Vowel_Preserve_Vowel_After_L(save_current))
            {
                return;
            }
            else
            {
                MetaphAdd("L");
            }

        }

        private void Encode_M()
        {
            if ((Encode_Silent_M_At_Beginning()
                        || (Encode_MR_And_MRS()
                        || (Encode_MAC() || Encode_MPT()))))
            {
                return;
            }

            //  Silent 'B' should really be handled
            //  under 'B", not here under 'M'!
            Encode_MB();
            MetaphAdd("M");
        }

        private bool Encode_Silent_M_At_Beginning()
        {
            // skip these when at start of word
            if (((m_current == 0)
                        && StringAt(m_current, 2, "MN", "")))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_MR_And_MRS()
        {
            if (((m_current == 0)
                        && StringAt(m_current, 2, "MR", "")))
            {
                //  exceptions for "Mr." and "Mrs."
                if (((m_length == 2)
                            && StringAt(m_current, 2, "MR", "")))
                {
                    if (m_encodeVowels)
                    {
                        MetaphAdd("MASTAR");
                    }
                    else
                    {
                        MetaphAdd("MSTR");
                    }

                    m_current += 2;
                    return true;
                }
                else if (((m_length == 3)
                            && StringAt(m_current, 3, "MRS", "")))
                {
                    if (m_encodeVowels)
                    {
                        MetaphAdd("MASAS");
                    }
                    else
                    {
                        MetaphAdd("MSS");
                    }

                    m_current += 3;
                    return true;
                }

            }

            return false;
        }

        private bool Encode_MAC()
        {
            // should only find Irish and
            // Scottish names e.g. 'Macintosh'
            if ((m_current == 0) && (StringAt(0, 7, "MACIVER", "MACEWEN", "") || StringAt(0, 8, "MACELROY", "MACILROY", "")
                    || StringAt(0, 9, "MACINTOSH", "") || StringAt(0, 2, "MC", "")))
            {
                if (m_encodeVowels)
                {
                    MetaphAdd("MAK");
                }
                else
                {
                    MetaphAdd("MK");
                }

                if (StringAt(0, 2, "MC", ""))
                {
                    if (StringAt((m_current + 2), 1, "K", "G", "Q", "")
                            // watch out for e.g. "McGeorge"
                            && !StringAt((m_current + 2), 4, "GEOR", ""))
                    {
                        m_current += 3;
                    }
                    else
                    {
                        m_current += 2;
                    }
                }
                else
                {
                    m_current += 3;
                }

                return true;
            }

            return false;
        }

        private bool Encode_MPT()
        {
            if ((StringAt((m_current - 2), 8, "COMPTROL", "") || StringAt((m_current - 4), 7, "ACCOMPT", "")))
            {
                MetaphAdd("N");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Test_Silent_MB_1()
        {
            //  e.g. "LAMB", "COMB", "LIMB", "DUMB", "BOMB"
            //  Handle combining roots first
            if ((((m_current == 3)
                        && StringAt((m_current - 3), 5, "THUMB", ""))
                        || ((m_current == 2)
                        && StringAt((m_current - 2), 4, "DUMB", "BOMB", "DAMN", "LAMB", "NUMB", "TOMB", ""))))
            {
                return true;
            }

            return false;
        }

        private bool Test_Pronounced_MB()
        {
            if ((StringAt((m_current - 2), 6, "NUMBER", "")
                        || ((StringAt((m_current + 2), 1, "A", "")
                        && !StringAt((m_current - 2), 7, "DUMBASS", ""))
                        || (StringAt((m_current + 2), 1, "O", "") || StringAt((m_current - 2), 6, "LAMBEN", "LAMBER", "LAMBET", "TOMBIG", "LAMBRE", "")))))
            {
                return true;
            }

            return false;
        }

        private bool Test_Silent_MB_2()
        {
            // 'M' is the current letter
            if ((CharAt(m_current + 1) == 'B') && (m_current > 1) && (((m_current + 1) == m_last)
                    // other situations where "-MB-" is at end of root
                    // but not at end of word. The tests are for standard
                    // noun suffixes.
                    // e.g. "climbing" => KLMNK
                    || StringAt((m_current + 2), 3, "ING", "ABL", "") || StringAt((m_current + 2), 4, "LIKE", "")
                    || ((CharAt(m_current + 2) == 'S') && ((m_current + 2) == m_last))
                    || StringAt((m_current - 5), 7, "BUNCOMB", "")
                    // e.g. "bomber",
                    || (StringAt((m_current + 2), 2, "ED", "ER", "") && ((m_current + 3) == m_last)
                            && (StringAt(0, 5, "CLIMB", "PLUMB", "")
                                    // e.g. "beachcomber"
                                    || !StringAt((m_current - 1), 5, "IMBER", "AMBER", "EMBER", "UMBER", ""))
                            // exceptions
                            && !StringAt((m_current - 2), 6, "CUMBER", "SOMBER", ""))))
            {
                return true;
            }

            return false;
        }

        private bool Test_Pronounced_MB_2()
        {
            //  e.g. "bombastic", "umbrage", "flamboyant"
            if ((StringAt((m_current - 1), 5, "OMBAS", "OMBAD", "UMBRA", "") || StringAt((m_current - 3), 4, "FLAM", "")))
            {
                return true;
            }

            return false;
        }

        private bool Test_MN()
        {
            if (((CharAt((m_current + 1)) == 'N')
                        && (((m_current + 1)
                        == m_last)
                        || ((StringAt((m_current + 2), 3, "ING", "EST", "")
                        && ((m_current + 4)
                        == m_last))
                        || (((CharAt((m_current + 2)) == 'S')
                        && ((m_current + 2)
                        == m_last))
                        || ((StringAt((m_current + 2), 2, "LY", "ER", "ED", "")
                        && ((m_current + 3)
                        == m_last))
                        || (StringAt((m_current - 2), 9, "DAMNEDEST", "") || StringAt((m_current - 5), 9, "GODDAMNIT", ""))))))))
            {
                return true;
            }

            return false;
        }

        private void Encode_MB()
        {
            if (Test_Silent_MB_1())
            {
                if (Test_Pronounced_MB())
                {
                    m_current++;
                }
                else
                {
                    m_current += 2;
                }

            }
            else if (Test_Silent_MB_2())
            {
                if (Test_Pronounced_MB_2())
                {
                    m_current++;
                }
                else
                {
                    m_current += 2;
                }

            }
            else if (Test_MN())
            {
                m_current += 2;
            }
            else
            {
                //  eat redundant 'M'
                if ((CharAt((m_current + 1)) == 'M'))
                {
                    m_current += 2;
                }
                else
                {
                    m_current++;
                }

            }

        }

        private void Encode_N()
        {
            if (Encode_NCE())
            {
                return;
            }

            // eat redundant 'N'
            if (CharAt(m_current + 1) == 'N')
            {
                m_current += 2;
            }
            else
            {
                m_current++;
            }

            if (!StringAt((m_current - 3), 8, "MONSIEUR", "")
                    // e.g. "aloneness",
                    && !StringAt((m_current - 3), 6, "NENESS", ""))
            {
                MetaphAdd("N");
            }
        }

        private bool Encode_NCE()
        {
            // 'acceptance', 'accountancy'
            if ((StringAt((m_current + 1), 1, "C", "S", "")
                        && (StringAt((m_current + 2), 1, "E", "Y", "I", "")
                        && (((m_current + 2)
                        == m_last)
                        || (((m_current + 3)
                        == m_last)
                        && (CharAt((m_current + 3)) == 'S'))))))
            {
                MetaphAdd("NTS");
                m_current += 2;
                return true;
            }

            return false;
        }

        private void Encode_P()
        {
            if ((Encode_Silent_P_At_Beginning()
                        || (Encode_PT()
                        || (Encode_PH()
                        || (Encode_PPH()
                        || (Encode_RPS()
                        || (Encode_COUP()
                        || (Encode_PNEUM()
                        || (Encode_PSYCH() || Encode_PSALM())))))))))
            {
                return;
            }

            Encode_PB();
            MetaphAdd("P");
        }

        private bool Encode_Silent_P_At_Beginning()
        {
            // skip these when at start of word
            if (((m_current == 0)
                        && StringAt(m_current, 2, "PN", "PF", "PS", "PT", "")))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_PT()
        {
            //  'pterodactyl', 'receipt', 'asymptote'
            if ((CharAt((m_current + 1)) == 'T'))
            {
                if ((((m_current == 0)
                            && StringAt(m_current, 5, "PTERO", ""))
                            || (StringAt((m_current - 5), 7, "RECEIPT", "") || StringAt((m_current - 4), 8, "ASYMPTOT", ""))))
                {
                    MetaphAdd("T");
                    m_current += 2;
                    return true;
                }

            }

            return false;
        }

        private bool Encode_PH()
        {
            if ((CharAt((m_current + 1)) == 'H'))
            {
                //  'PH' silent in these contexts
                if ((StringAt(m_current, 9, "PHTHALEIN", "")
                            || (((m_current == 0)
                            && StringAt(m_current, 4, "PHTH", ""))
                            || StringAt((m_current - 3), 10, "APOPHTHEGM", ""))))
                {
                    MetaphAdd("0");
                    m_current += 4;
                }

                //  combining forms
                // 'shepherd', 'upheaval', 'cup-holder'
                if (((m_current > 0)
                            && ((StringAt((m_current + 2), 3, "EAD", "OLE", "ELD", "ILL", "OLD", "EAP", "ERD", "ARD", "ANG", "ORN", "EAV", "ART", "")
                            || (StringAt((m_current + 2), 4, "OUSE", "")
                            || ((StringAt((m_current + 2), 2, "AM", "")
                            && !StringAt((m_current - 1), 5, "LPHAM", ""))
                            || (StringAt((m_current + 2), 5, "AMMER", "AZARD", "UGGER", "") || StringAt((m_current + 2), 6, "OLSTER", "")))))
                            && !StringAt((m_current - 3), 5, "LYMPH", "NYMPH", ""))))
                {
                    MetaphAdd("P");
                    AdvanceCounter(3, 2);
                }
                else
                {
                    MetaphAdd("F");
                    m_current += 2;
                }

                return true;
            }

            return false;
        }

        private bool Encode_PPH()
        {
            //  'Sappho'
            if (((CharAt((m_current + 1)) == 'P')
                        && (((m_current + 2)
                        < m_length)
                        && (CharAt((m_current + 2)) == 'H'))))
            {
                MetaphAdd("F");
                m_current += 3;
                return true;
            }

            return false;
        }

        private bool Encode_RPS()
        {
            // '-corps-', 'corpsman'
            if ((StringAt((m_current - 3), 5, "CORPS", "")
                        && !StringAt((m_current - 3), 6, "CORPSE", "")))
            {
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_COUP()
        {
            // 'coup'
            if (((m_current == m_last)
                        && (StringAt((m_current - 3), 4, "COUP", "")
                        && !StringAt((m_current - 5), 6, "RECOUP", ""))))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_PNEUM()
        {
            // '-pneum-'
            if (StringAt((m_current + 1), 4, "NEUM", ""))
            {
                MetaphAdd("N");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_PSYCH()
        {
            // '-psych-'
            if (StringAt((m_current + 1), 4, "SYCH", ""))
            {
                if (m_encodeVowels)
                {
                    MetaphAdd("SAK");
                }
                else
                {
                    MetaphAdd("SK");
                }

                m_current += 5;
                return true;
            }

            return false;
        }

        private bool Encode_PSALM()
        {
            // '-psalm-'
            if (StringAt((m_current + 1), 4, "SALM", ""))
            {
                //  go ahead and encode entire word
                if (m_encodeVowels)
                {
                    MetaphAdd("SAM");
                }
                else
                {
                    MetaphAdd("SM");
                }

                m_current += 5;
                return true;
            }

            return false;
        }

        private void Encode_PB()
        {
            //  e.g. "Campbell", "raspberry"
            //  eat redundant 'P' or 'B'
            if (StringAt((m_current + 1), 1, "P", "B", ""))
            {
                m_current += 2;
            }
            else
            {
                m_current++;
            }

        }

        private void Encode_Q()
        {
            //  current pinyin
            if (StringAt(m_current, 3, "QIN", ""))
            {
                MetaphAdd("X");
                m_current++;
                return;
            }

            //  eat redundant 'Q'
            if ((CharAt((m_current + 1)) == 'Q'))
            {
                m_current += 2;
            }
            else
            {
                m_current++;
            }

            MetaphAdd("K");
        }

        private void Encode_R()
        {
            if (Encode_RZ())
            {
                return;
            }

            if (!Test_Silent_R())
            {
                if (!Encode_Vowel_RE_Transposition())
                {
                    MetaphAdd("R");
                }

            }

            //  eat redundant 'R'; also skip 'S' as well as 'R' in "poitiers"
            if (((CharAt((m_current + 1)) == 'R')
                        || StringAt((m_current - 6), 8, "POITIERS", "")))
            {
                m_current += 2;
            }
            else
            {
                m_current++;
            }

        }

        private bool Encode_RZ()
        {
            if ((StringAt((m_current - 2), 4, "GARZ", "KURZ", "MARZ", "MERZ", "HERZ", "PERZ", "WARZ", "")
                        || (StringAt(m_current, 5, "RZANO", "RZOLA", "") || StringAt((m_current - 1), 4, "ARZA", "ARZN", ""))))
            {
                return false;
            }

            //  'yastrzemski' usually has 'z' silent in
            //  United States, but should get 'X' in Poland
            if (StringAt((m_current - 4), 11, "YASTRZEMSKI", ""))
            {
                MetaphAdd("R", "X");
                m_current += 2;
                return true;
            }

            //  'BRZEZINSKI' gets two pronunciations
            //  in the united states, neither of which
            //  are authentically polish
            if (StringAt((m_current - 1), 10, "BRZEZINSKI", ""))
            {
                MetaphAdd("RS", "RJ");
                //  skip over 2nd 'Z'
                m_current += 4;
                return true;
            }

            //  'z' in 'rz after voiceless consonant gets 'X'
            //  in alternate polish style pronunciation
            if ((StringAt((m_current - 1), 3, "TRZ", "PRZ", "KRZ", "")
                        || (StringAt(m_current, 2, "RZ", "")
                        && (IsVowel((m_current - 1))
                        || (m_current == 0)))))
            {
                MetaphAdd("RS", "X");
                m_current += 2;
                return true;
            }

            //  'z' in 'rz after voiceled consonant, vowel, or at
            //  beginning gets 'J' in alternate polish style pronunciation
            if (StringAt((m_current - 1), 3, "BRZ", "DRZ", "GRZ", ""))
            {
                MetaphAdd("RS", "J");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Test_Silent_R()
        {
            // test cases where 'R' is silent, either because the
            // word is from the French or because it is no longer pronounced.
            // e.g. "rogier", "monsieur", "suburban"
            if (((m_current == m_last)
                    // reliably French word ending
                    && StringAt((m_current - 2), 3, "IER", "")
                    // e.g. "metier"
                    && (StringAt((m_current - 5), 3, "MET", "VIV", "LUC", "")
                            // e.g. "Cartier", "bustier"
                            || StringAt((m_current - 6), 4, "CART", "DOSS", "FOUR", "OLIV", "BUST", "DAUM", "ATEL", "SONN",
                                    "CORM", "MERC", "PELT", "POIR", "BERN", "FORT", "GREN", "SAUC", "GAGN", "GAUT", "GRAN",
                                    "FORC", "MESS", "LUSS", "MEUN", "POTH", "HOLL", "CHEN", "")
                            // e.g. "croupier"
                            || StringAt((m_current - 7), 5, "CROUP", "TORCH", "CLOUT", "FOURN", "GAUTH", "TROTT", "DEROS",
                                    "CHART", "")
                            // e.g. "chevalier"
                            || StringAt((m_current - 8), 6, "CHEVAL", "LAVOIS", "PELLET", "SOMMEL", "TREPAN", "LETELL",
                                    "COLOMB", "")
                            || StringAt((m_current - 9), 7, "CHARCUT", "")
                            || StringAt((m_current - 10), 8, "CHARPENT", "")))
                    || StringAt((m_current - 2), 7, "SURBURB", "WORSTED", "")
                    || StringAt((m_current - 2), 9, "WORCESTER", "") || StringAt((m_current - 7), 8, "MONSIEUR", "")
                    || StringAt((m_current - 6), 8, "POITIERS", ""))
            {
                return true;
            }

            return false;
        }

        private bool Encode_Vowel_RE_Transposition()
        {
            //  -re inversion is just like
            //  -le inversion
            //  e.g. "fibre" => FABAR or "centre" => SANTAR
            if ((m_encodeVowels
                        && ((CharAt((m_current + 1)) == 'E')
                        && ((m_length > 3)
                        && (!StringAt(0, 5, "OUTRE", "LIBRE", "ANDRE", "")
                        && (!(StringAt(0, 4, "FRED", "TRES", "")
                        && (m_length == 4))
                        && (!StringAt((m_current - 2), 5, "LDRED", "LFRED", "NDRED", "NFRED", "NDRES", "TRES", "IFRED", "")
                        && (!IsVowel((m_current - 1))
                        && (((m_current + 1)
                        == m_last)
                        || (((m_current + 2)
                        == m_last)
                        && StringAt((m_current + 2), 1, "D", "S", "")))))))))))
            {
                MetaphAdd("AR");
                return true;
            }

            return false;
        }

        private void Encode_S()
        {
            if ((Encode_SKJ()
                        || (Encode_Special_SW()
                        || (Encode_SJ()
                        || (Encode_Silent_French_S_Final()
                        || (Encode_Silent_French_S_Internal()
                        || (Encode_ISL()
                        || (Encode_STL()
                        || (Encode_Christmas()
                        || (Encode_STHM()
                        || (Encode_ISTEN()
                        || (Encode_Sugar()
                        || (Encode_SH()
                        || (Encode_SCH()
                        || (Encode_SUR()
                        || (Encode_SU()
                        || (Encode_SSIO()
                        || (Encode_SS()
                        || (Encode_SIA()
                        || (Encode_SIO()
                        || (Encode_Anglicisations()
                        || (Encode_SC()
                        || (Encode_SEA_SUI_SIER() || Encode_SEA())))))))))))))))))))))))
            {
                return;
            }

            MetaphAdd("S");
            if ((StringAt((m_current + 1), 1, "S", "Z", "")
                        && !StringAt((m_current + 1), 2, "SH", "")))
            {
                m_current += 2;
            }
            else
            {
                m_current++;
            }

        }

        private bool Encode_Special_SW()
        {
            if ((m_current == 0))
            {
                //  
                if (Names_Beginning_With_SW_That_Get_Alt_SV())
                {
                    MetaphAdd("S", "SV");
                    m_current += 2;
                    return true;
                }

                //  
                if (Names_Beginning_With_SW_That_Get_Alt_XV())
                {
                    MetaphAdd("S", "XV");
                    m_current += 2;
                    return true;
                }

            }

            return false;
        }

        private bool Encode_SKJ()
        {
            //  Scandinavian
            if ((StringAt(m_current, 4, "SKJO", "SKJU", "") && IsVowel((m_current + 3))))
            {
                MetaphAdd("X");
                m_current += 3;
                return true;
            }

            return false;
        }

        private bool Encode_SJ()
        {
            if (StringAt(0, 2, "SJ", ""))
            {
                MetaphAdd("X");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_Silent_French_S_Final()
        {
            //  "Louis" is an exception because it gets two pronunciations
            if ((StringAt(0, 5, "LOUIS", "")
                        && (m_current == m_last)))
            {
                MetaphAdd("S", "");
                m_current++;
                return true;
            }

            //  French words familiar to Americans where final s is silent
            if ((((m_current == m_last)
                        && (StringAt(0, 4, "YVES", "")
                        || ((StringAt(0, 4, "HORS", "")
                        && (m_current == 3))
                        || (StringAt((m_current - 4), 5, "CAMUS", "YPRES", "")
                        || (StringAt((m_current - 5), 6, "MESNES", "DEBRIS", "BLANCS", "INGRES", "CANNES", "")
                        || (StringAt((m_current - 6), 7, "CHABLIS", "APROPOS", "JACQUES", "ELYSEES", "OEUVRES", "GEORGES", "DESPRES", "")
                        || (StringAt(0, 8, "ARKANSAS", "FRANCAIS", "CRUDITES", "BRUYERES", "")
                        || (StringAt(0, 9, "DESCARTES", "DESCHUTES", "DESCHAMPS", "DESROCHES", "DESCHENES", "")
                        || (StringAt(0, 10, "RENDEZVOUS", "") || StringAt(0, 11, "CONTRETEMPS", "DESLAURIERS", ""))))))))))
                        || ((m_current == m_last)
                        && (StringAt((m_current - 2), 2, "AI", "OI", "UI", "")
                        && !StringAt(0, 4, "LOIS", "LUIS", "")))))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_Silent_French_S_Internal()
        {
            //  French words familiar to Americans where internal s is silent
            if ((StringAt((m_current - 2), 9, "DESCARTES", "")
                        || (StringAt((m_current - 2), 7, "DESCHAM", "DESPRES", "DESROCH", "DESROSI", "DESJARD", "DESMARA", "DESCHEN", "DESHOTE", "DESLAUR", "")
                        || (StringAt((m_current - 2), 6, "MESNES", "")
                        || (StringAt((m_current - 5), 8, "DUQUESNE", "DUCHESNE", "")
                        || (StringAt((m_current - 7), 10, "BEAUCHESNE", "")
                        || (StringAt((m_current - 3), 7, "FRESNEL", "")
                        || (StringAt((m_current - 3), 9, "GROSVENOR", "")
                        || (StringAt((m_current - 4), 10, "LOUISVILLE", "") || StringAt((m_current - 7), 10, "ILLINOISAN", ""))))))))))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_ISL()
        {
            // special cases 'island', 'isle', 'carlisle', 'carlysle'
            if (((StringAt((m_current - 2), 4, "LISL", "LYSL", "AISL", "")
                        && !StringAt((m_current - 3), 7, "PAISLEY", "BAISLEY", "ALISLAM", "ALISLAH", "ALISLAA", ""))
                        || ((m_current == 1)
                        && ((StringAt((m_current - 1), 4, "ISLE", "") || StringAt((m_current - 1), 5, "ISLAN", ""))
                        && !StringAt((m_current - 1), 5, "ISLEY", "ISLER", "")))))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_STL()
        {
            // 'hustle', 'bustle', 'whistle'
            if ((StringAt(m_current, 4, "STLE", "STLI", "") && !StringAt((m_current + 2), 4, "LESS", "LIKE", "LINE", ""))
                    || StringAt((m_current - 3), 7, "THISTLY", "BRISTLY", "GRISTLY", "")
                    // e.g. "corpuscle"
                    || StringAt((m_current - 1), 5, "USCLE", ""))
            {
                // KRISTEN, KRYSTLE, CRYSTLE, KRISTLE all pronounce the 't'
                // also, exceptions where "-LING" is a nominalizing suffix
                if (StringAt(0, 7, "KRISTEN", "KRYSTLE", "CRYSTLE", "KRISTLE", "")
                        || StringAt(0, 11, "CHRISTENSEN", "CHRISTENSON", "")
                        || StringAt((m_current - 3), 9, "FIRSTLING", "")
                        || StringAt((m_current - 2), 8, "NESTLING", "WESTLING", ""))
                {
                    MetaphAdd("ST");
                    m_current += 2;
                }
                else
                {
                    if (m_encodeVowels && (CharAt(m_current + 3) == 'E') && (CharAt(m_current + 4) != 'R')
                            && !StringAt((m_current + 3), 4, "ETTE", "ETTA", "")
                            && !StringAt((m_current + 3), 2, "EY", ""))
                    {
                        MetaphAdd("SAL");
                        flag_AL_inversion = true;
                    }
                    else
                    {
                        MetaphAdd("SL");
                    }
                    m_current += 3;
                }
                return true;
            }

            return false;
        }

        private bool Encode_Christmas()
        {
            // 'Christmas'
            if (StringAt((m_current - 4), 8, "CHRISTMA", ""))
            {
                MetaphAdd("SM");
                m_current += 3;
                return true;
            }

            return false;
        }

        private bool Encode_STHM()
        {
            // 'asthma', 'isthmus'
            if (StringAt(m_current, 4, "STHM", ""))
            {
                MetaphAdd("SM");
                m_current += 4;
                return true;
            }

            return false;
        }

        private bool Encode_ISTEN()
        {
            //  't' is silent in verb, pronounced in name
            if (StringAt(0, 8, "CHRISTEN", ""))
            {
                //  the word itself
                if ((RootOrInflections(m_inWord, "CHRISTEN") || StringAt(0, 11, "CHRISTENDOM", "")))
                {
                    MetaphAdd("S", "ST");
                }
                else
                {
                    //  e.g. 'Christenson', 'christene'                
                    MetaphAdd("ST");
                }

                m_current += 2;
                return true;
            }

            // e.g. 'glisten', 'listen'
            if ((StringAt((m_current - 2), 6, "LISTEN", "RISTEN", "HASTEN", "FASTEN", "MUSTNT", "") || StringAt((m_current - 3), 7, "MOISTEN", "")))
            {
                MetaphAdd("S");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_Sugar()
        {
            // special case 'sugar-'
            if (StringAt(m_current, 5, "SUGAR", ""))
            {
                MetaphAdd("X");
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_SH()
        {
            if (StringAt(m_current, 2, "SH", ""))
            {
                // exception
                if (StringAt((m_current - 2), 8, "CASHMERE", ""))
                {
                    MetaphAdd("J");
                    m_current += 2;
                    return true;
                }

                // combining forms, e.g. 'clotheshorse', 'woodshole'
                if ((m_current > 0)
                        // e.g. "mishap"
                        && ((StringAt((m_current + 1), 3, "HAP", "") && ((m_current + 3) == m_last))
                                // e.g. "hartsheim", "clotheshorse"
                                || StringAt((m_current + 1), 4, "HEIM", "HOEK", "HOLM", "HOLZ", "HOOD", "HEAD", "HEID",
                                        "HAAR", "HORS", "HOLE", "HUND", "HELM", "HAWK", "HILL", "")
                                // e.g. "dishonor"
                                || StringAt((m_current + 1), 5, "HEART", "HATCH", "HOUSE", "HOUND", "HONOR", "")
                                // e.g. "mishear"
                                || (StringAt((m_current + 2), 3, "EAR", "") && ((m_current + 4) == m_last))
                                // e.g. "hartshorn"
                                || (StringAt((m_current + 2), 3, "ORN", "") && !StringAt((m_current - 2), 7, "UNSHORN", ""))
                                // e.g. "newshour" but not "bashour", "manshour"
                                || (StringAt((m_current + 1), 4, "HOUR", "") && !(StringAt(0, 7, "BASHOUR", "")
                                        || StringAt(0, 8, "MANSHOUR", "") || StringAt(0, 6, "ASHOUR", "")))
                                // e.g. "dishonest", "grasshopper"
                                || StringAt((m_current + 2), 5, "ARMON", "ONEST", "ALLOW", "OLDER", "OPPER", "EIMER",
                                        "ANDLE", "ONOUR", "")
                                // e.g. "dishabille", "transhumance"
                                || StringAt((m_current + 2), 6, "ABILLE", "UMANCE", "ABITUA", "")))
                {
                    if (!StringAt((m_current - 1), 1, "S", ""))
                        MetaphAdd("S");
                }
                else
                {
                    MetaphAdd("X");
                }

                m_current += 2;
                return true;
            }

            return false;
        }


        private bool Encode_SCH()
        {
            // these words were combining forms many centuries ago
            if (StringAt((m_current + 1), 2, "CH", ""))
            {
                if ((m_current > 0)
                        // e.g. "mischief", "escheat"
                        && (StringAt((m_current + 3), 3, "IEF", "EAT", "")
                                // e.g. "mischance"
                                || StringAt((m_current + 3), 4, "ANCE", "ARGE", "")
                                // e.g. "eschew"
                                || StringAt(0, 6, "ESCHEW", "")))
                {
                    MetaphAdd("S");
                    m_current++;
                    return true;
                }

                // Schlesinger's rule
                // dutch, danish, Italian, Greek origin, e.g. "school", "schooner", "schiavone",
                // "schiz-"
                if ((StringAt((m_current + 3), 2, "OO", "ER", "EN", "UY", "ED", "EM", "IA", "IZ", "IS", "OL", "")
                        && !StringAt(m_current, 6, "SCHOLT", "SCHISL", "SCHERR", ""))
                        || StringAt((m_current + 3), 3, "ISZ", "")
                        || (StringAt((m_current - 1), 6, "ESCHAT", "ASCHIN", "ASCHAL", "ISCHAE", "ISCHIA", "")
                                && !StringAt((m_current - 2), 8, "FASCHING", ""))
                        || (StringAt((m_current - 1), 5, "ESCHI", "") && ((m_current + 3) == m_last))
                        || (CharAt(m_current + 3) == 'Y'))
                {
                    // e.g. "schermerhorn", "schenker", "schistose"
                    if (StringAt((m_current + 3), 2, "ER", "EN", "IS", "")
                            && (((m_current + 4) == m_last) || StringAt((m_current + 3), 3, "ENK", "ENB", "IST", "")))
                    {
                        MetaphAdd("X", "SK");
                    }
                    else
                    {
                        MetaphAdd("SK");
                    }
                    m_current += 3;
                    return true;
                }
                else
                {
                    MetaphAdd("X");
                    m_current += 3;
                    return true;
                }
            }

            return false;
        }


        private bool Encode_SUR()
        {
            //  'erasure', 'usury'
            if (StringAt((m_current + 1), 3, "URE", "URA", "URY", ""))
            {
                // 'sure', 'ensure'
                if (((m_current == 0)
                            || (StringAt((m_current - 1), 1, "N", "K", "") || StringAt((m_current - 2), 2, "NO", ""))))
                {
                    MetaphAdd("X");
                }
                else
                {
                    MetaphAdd("J");
                }

                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_SU()
        {
            // 'sensuous', 'consensual'
            if ((StringAt((m_current + 1), 2, "UO", "UA", "")
                        && (m_current != 0)))
            {
                //  exceptions e.g. "persuade"
                if (StringAt((m_current - 1), 4, "RSUA", ""))
                {
                    MetaphAdd("S");
                }

                //  exceptions e.g. "casual"
                if (IsVowel((m_current - 1)))
                {
                    MetaphAdd("J", "S");
                }
                else
                {
                    MetaphAdd("X", "S");
                }

                AdvanceCounter(3, 1);
                return true;
            }

            return false;
        }

        private bool Encode_SSIO()
        {
            if (StringAt((m_current + 1), 4, "SION", ""))
            {
                // "abscission"
                if (StringAt((m_current - 2), 2, "CI", ""))
                {
                    MetaphAdd("J");
                }

                // 'mission'
                if (IsVowel((m_current - 1)))
                {
                    MetaphAdd("X");
                }

                AdvanceCounter(4, 2);
                return true;
            }

            return false;
        }

        private bool Encode_SS()
        {
            // e.g. "Russian", "pressure"
            if (StringAt((m_current - 1), 5, "USSIA", "ESSUR", "ISSUR", "ISSUE", "")
                    // e.g. "Hessian", "assurance"
                    || StringAt((m_current - 1), 6, "ESSIAN", "ASSURE", "ASSURA", "ISSUAB", "ISSUAN", "ASSIUS", ""))
            {
                MetaphAdd("X");
                AdvanceCounter(3, 2);
                return true;
            }

            return false;
        }

        private bool Encode_SIA()
        {
            //  e.g. "controversial", also "fuchsia", "ch" is silent
            if ((StringAt((m_current - 2), 5, "CHSIA", "") || StringAt((m_current - 1), 5, "RSIAL", "")))
            {
                MetaphAdd("X");
                AdvanceCounter(3, 1);
                return true;
            }

            //  names generally get 'X' where terms, e.g. "aphasia" get 'J'
            if (((StringAt(0, 6, "ALESIA", "ALYSIA", "ALISIA", "STASIA", "")
                        && ((m_current == 3)
                        && !StringAt(0, 9, "ANASTASIA", "")))
                        || (StringAt((m_current - 5), 9, "DIONYSIAN", "") || StringAt((m_current - 5), 8, "THERESIA", ""))))
            {
                MetaphAdd("X", "S");
                AdvanceCounter(3, 1);
                return true;
            }

            if (((StringAt(m_current, 3, "SIA", "")
                        && ((m_current + 2)
                        == m_last))
                        || ((StringAt(m_current, 4, "SIAN", "")
                        && ((m_current + 3)
                        == m_last))
                        || StringAt((m_current - 5), 9, "AMBROSIAL", ""))))
            {
                if (((IsVowel((m_current - 1)) || StringAt((m_current - 1), 1, "R", ""))
                            && !(StringAt(0, 5, "JAMES", "NICOS", "PEGAS", "PEPYS", "")
                            || (StringAt(0, 6, "HOBBES", "HOLMES", "JAQUES", "KEYNES", "")
                            || (StringAt(0, 7, "MALTHUS", "HOMOOUS", "")
                            || (StringAt(0, 8, "MAGLEMOS", "HOMOIOUS", "")
                            || (StringAt(0, 9, "LEVALLOIS", "TARDENOIS", "") || StringAt((m_current - 4), 5, "ALGES", ""))))))))
                {
                    MetaphAdd("J");
                }
                else
                {
                    MetaphAdd("S");
                }

                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_SIO()
        {
            //  special case, Irish name
            if (StringAt(0, 7, "SIOBHAN", ""))
            {
                MetaphAdd("X");
                AdvanceCounter(3, 1);
                return true;
            }

            if (StringAt((m_current + 1), 3, "ION", ""))
            {
                //  e.g. "vision", "version"
                if ((IsVowel((m_current - 1)) || StringAt((m_current - 2), 2, "ER", "UR", "")))
                {
                    MetaphAdd("J");
                }
                else
                {
                    MetaphAdd("X");
                }

                AdvanceCounter(3, 1);
                return true;
            }

            return false;
        }

        private bool Encode_Anglicisations()
        {
            // German & Anglicizations, e.g. 'smith' match 'Schmidt', 'snider' match 'Schneider'
            // also, -sz- in Slavic language altho in Hungarian it is pronounced 's'
            if ((((m_current == 0)
                        && StringAt((m_current + 1), 1, "M", "N", "L", ""))
                        || StringAt((m_current + 1), 1, "Z", "")))
            {
                MetaphAdd("S", "X");
                //  eat redundant 'Z'
                if (StringAt((m_current + 1), 1, "Z", ""))
                {
                    m_current += 2;
                }
                else
                {
                    m_current++;
                }

                return true;
            }

            return false;
        }

        private bool Encode_SC()
        {
            if (StringAt(m_current, 2, "SC", ""))
            {
                // exception 'viscount'
                if (StringAt((m_current - 2), 8, "VISCOUNT", ""))
                {
                    m_current += 1;
                    return true;
                }

                // encode "-SC<front vowel>-"
                if (StringAt((m_current + 2), 1, "I", "E", "Y", ""))
                {
                    // e.g. "conscious"
                    if (StringAt((m_current + 2), 4, "IOUS", "")
                            // e.g. "prosciutto"
                            || StringAt((m_current + 2), 3, "IUT", "") || StringAt((m_current - 4), 9, "OMNISCIEN", "")
                            // e.g. "conscious"
                            || StringAt((m_current - 3), 8, "CONSCIEN", "CRESCEND", "CONSCION", "")
                            || StringAt((m_current - 2), 6, "FASCIS", ""))
                    {
                        MetaphAdd("X");
                    }
                    else if (StringAt(m_current, 7, "SCEPTIC", "SCEPSIS", "")
                            || StringAt(m_current, 5, "SCIVV", "SCIRO", "")
                            // commonly pronounced this way in u.s.
                            || StringAt(m_current, 6, "SCIPIO", "") || StringAt((m_current - 2), 10, "PISCITELLI", ""))
                    {
                        MetaphAdd("SK");
                    }
                    else
                    {
                        MetaphAdd("S");
                    }
                    m_current += 2;
                    return true;
                }

                MetaphAdd("SK");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_SEA_SUI_SIER()
        {
            //  "nausea" by itself has => NJ as a more likely encoding. Other forms
            //  using "nausea-" (see Encode_SEA()) have X or S as more familiar pronunciations
            if (((StringAt((m_current - 3), 6, "NAUSEA", "")
                        && ((m_current + 2)
                        == m_last))
                        || (StringAt((m_current - 2), 5, "CASUI", "")
                        || (StringAt((m_current - 1), 5, "OSIER", "ASIER", "")
                        && !(StringAt(0, 6, "EASIER", "")
                        || (StringAt(0, 5, "OSIER", "") || StringAt((m_current - 2), 6, "ROSIER", "MOSIER", "")))))))
            {
                MetaphAdd("J", "X");
                AdvanceCounter(3, 1);
                return true;
            }

            return false;
        }

        private bool Encode_SEA()
        {
            if (((StringAt(0, 4, "SEAN", "")
                        && ((m_current + 3)
                        == m_last))
                        || (StringAt((m_current - 3), 6, "NAUSEO", "")
                        && !StringAt((m_current - 3), 7, "NAUSEAT", ""))))
            {
                MetaphAdd("X");
                AdvanceCounter(3, 1);
                return true;
            }

            return false;
        }

        void Encode_T()
        {
            if ((Encode_T_Initial()
                        || (Encode_TCH()
                        || (Encode_Silent_French_T()
                        || (Encode_TUN_TUL_TUA_TUO()
                        || (Encode_TUE_TEU_TEOU_TUL_TIE()
                        || (Encode_TUR_TIU_Suffixes()
                        || (Encode_TI()
                        || (Encode_TIENT()
                        || (Encode_TSCH()
                        || (Encode_TZSCH()
                        || (Encode_TH_Pronounced_Separately()
                        || (Encode_TTH() || Encode_TH())))))))))))))
            {
                return;
            }

            //  eat redundant 'T' or 'D'
            if (StringAt((m_current + 1), 1, "T", "D", ""))
            {
                m_current += 2;
            }
            else
            {
                m_current++;
            }

            MetaphAdd("T");
        }

        private bool Encode_T_Initial()
        {
            if ((m_current == 0))
            {
                //  Americans usually pronounce "tzar" as "zar"
                if (StringAt((m_current + 1), 3, "SAR", "ZAR", ""))
                {
                    m_current++;
                    return true;
                }

                //  old 'Ecole Francaise d'Extreme-Orient' Chinese pinyin where 'ts-' => 'X'
                if ((((m_length == 3)
                            && StringAt((m_current + 1), 2, "SO", "SA", "SU", ""))
                            || (((m_length == 4)
                            && StringAt((m_current + 1), 3, "SAO", "SAI", ""))
                            || ((m_length == 5)
                            && StringAt((m_current + 1), 4, "SING", "SANG", "")))))
                {
                    MetaphAdd("X");
                    AdvanceCounter(3, 2);
                    return true;
                }

                //  "TS<vowel>-" at start can be pronounced both with and without 'T'
                if ((StringAt((m_current + 1), 1, "S", "") && IsVowel((m_current + 2))))
                {
                    MetaphAdd("TS", "S");
                    AdvanceCounter(3, 2);
                    return true;
                }

                //  e.g. "Tjaarda"
                if (StringAt((m_current + 1), 1, "J", ""))
                {
                    MetaphAdd("X");
                    AdvanceCounter(3, 2);
                    return true;
                }

                //  cases where initial "TH-" is pronounced as T and not 0 ("th")
                if (((StringAt((m_current + 1), 2, "HU", "")
                            && (m_length == 3))
                            || (StringAt((m_current + 1), 3, "HAI", "HUY", "HAO", "")
                            || (StringAt((m_current + 1), 4, "HYME", "HYMY", "HANH", "") || StringAt((m_current + 1), 5, "HERES", "")))))
                {
                    MetaphAdd("T");
                    AdvanceCounter(3, 2);
                    return true;
                }

            }

            return false;
        }

        private bool Encode_TCH()
        {
            if (StringAt((m_current + 1), 2, "CH", ""))
            {
                MetaphAdd("X");
                m_current += 3;
                return true;
            }

            return false;
        }

        private bool Encode_Silent_French_T()
        {
            //  French silent T familiar to Americans
            if ((((m_current == m_last)
                        && StringAt((m_current - 4), 5, "MONET", "GENET", "CHAUT", ""))
                        || (StringAt((m_current - 2), 9, "POTPOURRI", "")
                        || (StringAt((m_current - 3), 9, "BOATSWAIN", "")
                        || (StringAt((m_current - 3), 8, "MORTGAGE", "")
                        || ((StringAt((m_current - 4), 5, "BERET", "BIDET", "FILET", "DEBUT", "DEPOT", "PINOT", "TAROT", "")
                        || (StringAt((m_current - 5), 6, "BALLET", "BUFFET", "CACHET", "CHALET", "ESPRIT", "RAGOUT", "GOULET", "CHABOT", "BENOIT", "")
                        || (StringAt((m_current - 6), 7, "GOURMET", "BOUQUET", "CROCHET", "CROQUET", "PARFAIT", "PINCHOT", "CABARET", "PARQUET", "RAPPORT", "TOUCHET", "COURBET", "DIDEROT", "")
                        || (StringAt((m_current - 7), 8, "ENTREPOT", "CABERNET", "DUBONNET", "MASSENET", "MUSCADET", "RICOCHET", "ESCARGOT", "") || StringAt((m_current - 8), 9, "SOBRIQUET", "CABRIOLET", "CASSOULET", "OUBRIQUET", "CAMEMBERT", "")))))
                        && !StringAt((m_current + 1), 2, "AN", "RY", "IC", "OM", "IN", "")))))))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_TUN_TUL_TUA_TUO()
        {
            // e.g. "fortune", "fortunate"
            if (StringAt((m_current - 3), 6, "FORTUN", "")
                    // e.g. "capitulate"
                    || (StringAt(m_current, 3, "TUL", "") && (IsVowel(m_current - 1) && IsVowel(m_current + 3)))
                    // e.g. "obituary", "barbituate"
                    || StringAt((m_current - 2), 5, "BITUA", "BITUE", "")
                    // e.g. "actual"
                    || ((m_current > 1) && StringAt(m_current, 3, "TUA", "TUO", "")))
            {
                MetaphAdd("X", "T");
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_TUE_TEU_TEOU_TUL_TIE()
        {
            // 'constituent', 'Pasteur'
            if (StringAt((m_current + 1), 4, "UENT", "") || StringAt((m_current - 4), 9, "RIGHTEOUS", "")
                    || StringAt((m_current - 3), 7, "STATUTE", "") || StringAt((m_current - 3), 7, "AMATEUR", "")
                    // e.g. "blastula", "Pasteur"
                    || (StringAt((m_current - 1), 5, "NTULE", "NTULA", "STULE", "STULA", "STEUR", ""))
                    // e.g. "statue"
                    || (((m_current + 2) == m_last) && StringAt(m_current, 3, "TUE", ""))
                    // e.g. "constituency"
                    || StringAt(m_current, 5, "TUENC", "")
                    // e.g. "statutory"
                    || StringAt((m_current - 3), 8, "STATUTOR", "")
                    // e.g. "patience"
                    || (((m_current + 5) == m_last) && StringAt(m_current, 6, "TIENCE", "")))
            {
                MetaphAdd("X", "T");
                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_TUR_TIU_Suffixes()
        {
            // 'adventure', 'musculature'
            if ((m_current > 0) && StringAt((m_current + 1), 3, "URE", "URA", "URI", "URY", "URO", "IUS", ""))
            {
                // exceptions e.g. 'tessitura', mostly from romance languages
                if ((StringAt((m_current + 1), 3, "URA", "URO", "")
                        // && !StringAt((m_current + 1), 4, "URIA", "")
                        && ((m_current + 3) == m_last)) && !StringAt((m_current - 3), 7, "VENTURA", "")
                        // e.g. "Khachaturian", "hematuria"
                        || StringAt((m_current + 1), 4, "URIA", ""))
                {
                    MetaphAdd("T");
                }
                else
                {
                    MetaphAdd("X", "T");
                }

                AdvanceCounter(2, 1);
                return true;
            }

            return false;
        }

        private bool Encode_TI()
        {
            // '-tio-', '-tia-', '-tiu-'
            // except combining forms where T already pronounced e.g 'Rooseveltian'
            if ((StringAt((m_current + 1), 2, "IO", "") && !StringAt((m_current - 1), 5, "ETIOL", ""))
                    || StringAt((m_current + 1), 3, "IAL", "") || StringAt((m_current - 1), 5, "RTIUM", "ATIUM", "")
                    || ((StringAt((m_current + 1), 3, "IAN", "") && (m_current > 0))
                            && !(StringAt((m_current - 4), 8, "FAUSTIAN", "")
                                    || StringAt((m_current - 5), 9, "PROUSTIAN", "")
                                    || StringAt((m_current - 2), 7, "TATIANA", "")
                                    || (StringAt((m_current - 3), 7, "KANTIAN", "GENTIAN", "")
                                            || StringAt((m_current - 8), 12, "ROOSEVELTIAN", "")))
                            || (((m_current + 2) == m_last) && StringAt(m_current, 3, "TIA", "")
                                    // exceptions to above rules where the pronunciation is usually X
                                    && !(StringAt((m_current - 3), 6, "HESTIA", "MASTIA", "")
                                            || StringAt((m_current - 2), 5, "OSTIA", "") || StringAt(0, 3, "TIA", "")
                                            || StringAt((m_current - 5), 8, "IZVESTIA", "")))
                            || StringAt((m_current + 1), 4, "IATE", "IATI", "IABL", "IATO", "IARY", "")
                            || StringAt((m_current - 5), 9, "CHRISTIAN", "")))
            {
                if (((m_current == 2) && StringAt(0, 4, "ANTI", "")) || StringAt(0, 5, "PATIO", "PITIA", "DUTIA", ""))
                {
                    MetaphAdd("T");
                }
                else if (StringAt((m_current - 4), 8, "EQUATION", ""))
                {
                    MetaphAdd("J");
                }
                else
                {
                    if (StringAt(m_current, 4, "TION", ""))
                    {
                        MetaphAdd("X");
                    }
                    else if (StringAt(0, 5, "KATIA", "LATIA", ""))
                    {
                        MetaphAdd("T", "X");
                    }
                    else
                    {
                        MetaphAdd("X", "T");
                    }
                }

                AdvanceCounter(3, 1);
                return true;
            }

            return false;
        }

        private bool Encode_TIENT()
        {
            //  e.g. 'patient'
            if (StringAt((m_current + 1), 4, "IENT", ""))
            {
                MetaphAdd("X", "T");
                AdvanceCounter(3, 1);
                return true;
            }

            return false;
        }

        private bool Encode_TSCH()
        {
            // 'Deutsch'
            if (StringAt(m_current, 4, "TSCH", "")
                    // combining forms in German where the 'T' is pronounced separately
                    && !StringAt((m_current - 3), 4, "WELT", "KLAT", "FEST", ""))
            {
                // pronounced the same as "ch" in "chit" => X
                MetaphAdd("X");
                m_current += 4;
                return true;
            }

            return false;
        }

        private bool Encode_TZSCH()
        {
            // 'Nietzsche'
            if (StringAt(m_current, 5, "TZSCH", ""))
            {
                MetaphAdd("X");
                m_current += 5;
                return true;
            }

            return false;
        }

        private bool Encode_TH_Pronounced_Separately()
        {
            // 'adulthood', 'bithead', 'apartheid'
            if (((m_current > 0)
                    && StringAt((m_current + 1), 4, "HOOD", "HEAD", "HEID", "HAND", "HILL", "HOLD", "HAWK", "HEAP", "HERD",
                            "HOLE", "HOOK", "HUNT", "HUMO", "HAUS", "HOFF", "HARD", "")
                    && !StringAt((m_current - 3), 5, "SOUTH", "NORTH", ""))
                    || StringAt((m_current + 1), 5, "HOUSE", "HEART", "HASTE", "HYPNO", "HEQUE", "")
                    // watch out for Greek root "-thallic"
                    || (StringAt((m_current + 1), 4, "HALL", "") && ((m_current + 4) == m_last)
                            && !StringAt((m_current - 3), 5, "SOUTH", "NORTH", ""))
                    || (StringAt((m_current + 1), 3, "HAM", "") && ((m_current + 3) == m_last)
                            && !(StringAt(0, 6, "GOTHAM", "WITHAM", "LATHAM", "")
                                    || StringAt(0, 7, "BENTHAM", "WALTHAM", "WORTHAM", "")
                                    || StringAt(0, 8, "GRANTHAM", "")))
                    || (StringAt((m_current + 1), 5, "HATCH", "")
                            && !((m_current == 0) || StringAt((m_current - 2), 8, "UNTHATCH", "")))
                    || StringAt((m_current - 3), 7, "WARTHOG", "")
                    // and some special cases where "-TH-" is usually pronounced 'T'
                    || StringAt((m_current - 2), 6, "ESTHER", "") || StringAt((m_current - 3), 6, "GOETHE", "")
                    || StringAt((m_current - 2), 8, "NATHALIE", ""))
            {
                // special case
                if (StringAt((m_current - 3), 7, "POSTHUM", ""))
                {
                    MetaphAdd("X");
                }
                else
                {
                    MetaphAdd("T");
                }
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_TTH()
        {
            //  'Matthew' vs. 'outthink'
            if (StringAt(m_current, 3, "TTH", ""))
            {
                if (StringAt((m_current - 2), 5, "MATTH", ""))
                {
                    MetaphAdd("0");
                }
                else
                {
                    MetaphAdd("T0");
                }

                m_current += 3;
                return true;
            }

            return false;
        }

        private bool Encode_TH()
        {
            if (StringAt(m_current, 2, "TH", ""))
            {
                // '-clothes-'
                if (StringAt((m_current - 3), 7, "CLOTHES", ""))
                {
                    //  vowel already encoded so skip right to S
                    m_current += 3;
                    return true;
                }

                // special case "Thomas", "Thames", "Beethoven" or Germanic words
                if ((StringAt((m_current + 2), 4, "OMAS", "OMPS", "OMPK", "OMSO", "OMSE", "AMES", "OVEN", "OFEN", "ILDA", "ILDE", "")
                            || ((StringAt(0, 4, "THOM", "")
                            && (m_length == 4))
                            || ((StringAt(0, 5, "THOMS", "")
                            && (m_length == 5))
                            || (StringAt(0, 4, "VAN ", "VON ", "") || StringAt(0, 3, "SCH", ""))))))
                {
                    MetaphAdd("T");
                }
                else
                {
                    //  give an 'etymological' 2nd
                    //  encoding for "smith"
                    if (StringAt(0, 2, "SM", ""))
                    {
                        MetaphAdd("0", "T");
                    }
                    else
                    {
                        MetaphAdd("0");
                    }

                }

                m_current += 2;
                return true;
            }

            return false;
        }

        void Encode_V()
        {
            //  eat redundant 'V'
            if ((CharAt((m_current + 1)) == 'V'))
            {
                m_current += 2;
            }
            else
            {
                m_current++;
            }

            MetaphAddExactApprox("V", "F");
        }

        void Encode_W()
        {
            if ((Encode_Silent_W_At_Beginning()
                        || (Encode_WITZ_WICZ()
                        || (Encode_WR()
                        || (Encode_Initial_W_Vowel()
                        || (Encode_WH() || Encode_Eastern_European_W()))))))
            {
                return;
            }

            //  e.g. 'Zimbabwe'
            if ((m_encodeVowels
                        && (StringAt(m_current, 2, "WE", "")
                        && ((m_current + 1)
                        == m_last))))
            {
                MetaphAdd("A");
            }

            // else skip it
            m_current++;
        }

        private bool Encode_Silent_W_At_Beginning()
        {
            // skip these when at start of word
            if (((m_current == 0)
                        && StringAt(m_current, 2, "WR", "")))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_WITZ_WICZ()
        {
            // polish e.g. 'filipowicz'
            if ((((m_current + 3)
                        == m_last)
                        && StringAt(m_current, 4, "WICZ", "WITZ", "")))
            {
                if (m_encodeVowels)
                {
                    if (((m_primary.Length > 0)
                                && (m_primary[(m_primary.Length - 1)] == 'A')))
                    {
                        MetaphAdd("TS", "FAX");
                    }
                    else
                    {
                        MetaphAdd("ATS", "FAX");
                    }

                }
                else
                {
                    MetaphAdd("TS", "FX");
                }

                m_current += 4;
                return true;
            }

            return false;
        }

        private bool Encode_WR()
        {
            // can also be in middle of word
            if (StringAt(m_current, 2, "WR", ""))
            {
                MetaphAdd("R");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_Initial_W_Vowel()
        {
            if (((m_current == 0)
                        && IsVowel((m_current + 1))))
            {
                // Witter should match Vitter
                if (Germanic_Or_Slavic_Name_Beginning_With_W())
                {
                    if (m_encodeVowels)
                    {
                        MetaphAddExactApprox("A", "VA", "A", "FA");
                    }
                    else
                    {
                        MetaphAddExactApprox("A", "V", "A", "F");
                    }

                }
                else
                {
                    MetaphAdd("A");
                }

                m_current++;
                //  don't encode vowels twice
                m_current = SkipVowels(m_current);
                return true;
            }

            return false;
        }

        private bool Encode_WH()
        {
            if (StringAt(m_current, 2, "WH", ""))
            {
                //  cases where it is pronounced as H
                //  e.g. 'who', 'whole'
                if (((CharAt((m_current + 2)) == 'O')
                            && !(StringAt((m_current + 2), 4, "OOSH", "")
                            || (StringAt((m_current + 2), 3, "OOP", "OMP", "ORL", "ORT", "") || StringAt((m_current + 2), 2, "OA", "OP", "")))))
                {
                    MetaphAdd("H");
                    AdvanceCounter(3, 2);
                    return true;
                }
                else
                {
                    //  combining forms, e.g. 'hollow-hearted', 'rawhide'
                    if ((StringAt((m_current + 2), 3, "IDE", "ARD", "EAD", "AWK", "ERD", "OOK", "AND", "OLE", "OOD", "")
                                || (StringAt((m_current + 2), 4, "EART", "OUSE", "OUND", "") || StringAt((m_current + 2), 5, "AMMER", ""))))
                    {
                        MetaphAdd("H");
                        m_current += 2;
                        return true;
                    }
                    else if ((m_current == 0))
                    {
                        MetaphAdd("A");
                        m_current += 2;
                        //  don't encode vowels twice
                        m_current = SkipVowels(m_current);
                        return true;
                    }

                }

                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_Eastern_European_W()
        {
            // Arnow should match Arnoff
            if ((((m_current == m_last)
                        && IsVowel((m_current - 1)))
                        || (StringAt((m_current - 1), 5, "EWSKI", "EWSKY", "OWSKI", "OWSKY", "")
                        || ((StringAt(m_current, 5, "WICKI", "WACKI", "")
                        && ((m_current + 4)
                        == m_last))
                        || ((StringAt(m_current, 4, "WIAK", "")
                        && ((m_current + 3)
                        == m_last))
                        || StringAt(0, 3, "SCH", ""))))))
            {
                MetaphAddExactApprox("", "V", "", "F");
                m_current++;
                return true;
            }

            return false;
        }

        void Encode_X()
        {
            if ((Encode_Initial_X()
                        || (Encode_Greek_X()
                        || (Encode_X_Special_Cases()
                        || (Encode_X_To_H()
                        || (Encode_X_Vowel() || Encode_French_X_Final()))))))
            {
                return;
            }

            //  eat redundant 'X' or other redundant cases
            if (StringAt((m_current + 1), 1, "X", "Z", "S", ""))
            {
                //  e.g. "excite", "exceed"
            }

            StringAt((m_current + 1), 2, "CI", "CE", "");
            m_current += 2;
            m_current++;
        }

        private bool Encode_Initial_X()
        {
            //  current Chinese pinyin spelling
            if ((StringAt(0, 3, "XIA", "XIO", "XIE", "") || StringAt(0, 2, "XU", "")))
            {
                MetaphAdd("X");
                m_current++;
                return true;
            }

            //  else
            if ((m_current == 0))
            {
                MetaphAdd("S");
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_Greek_X()
        {
            //  'xylophone', xylem', 'xanthoma', 'xeno-'
            if ((StringAt((m_current + 1), 3, "YLO", "YLE", "ENO", "") || StringAt((m_current + 1), 4, "ANTH", "")))
            {
                MetaphAdd("S");
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_X_Special_Cases()
        {
            //  'luxury'
            if (StringAt((m_current - 2), 5, "LUXUR", ""))
            {
                MetaphAddExactApprox("GJ", "KJ");
                m_current++;
                return true;
            }

            //  'texeira' Portuguese/Galician name
            if ((StringAt(0, 7, "TEXEIRA", "") || StringAt(0, 8, "TEIXEIRA", "")))
            {
                MetaphAdd("X");
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_X_To_H()
        {
            //  TODO: look for other Mexican Indian words
            //  where 'X' is usually pronounced this way
            if ((StringAt((m_current - 2), 6, "OAXACA", "") || StringAt((m_current - 3), 7, "QUIXOTE", "")))
            {
                MetaphAdd("H");
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_X_Vowel()
        {
            //  e.g. "sexual", "connexion" (British), "noxious"
            if (StringAt((m_current + 1), 3, "UAL", "ION", "IOU", ""))
            {
                MetaphAdd("KX", "KS");
                AdvanceCounter(3, 1);
                return true;
            }

            return false;
        }

        private bool Encode_French_X_Final()
        {
            // French e.g. "breaux", "paix"
            if (!((m_current == m_last)
                        && (StringAt((m_current - 3), 3, "IAU", "EAU", "IEU", "") || StringAt((m_current - 2), 2, "AI", "AU", "OU", "OI", "EU", ""))))
            {
                MetaphAdd("KS");
            }

            return false;
        }

        void Encode_Z()
        {
            if ((Encode_ZZ()
                        || (Encode_ZU_ZIER_ZS()
                        || (Encode_French_EZ() || Encode_German_Z()))))
            {
                return;
            }

            if (Encode_ZH())
            {
                return;
            }
            else
            {
                MetaphAdd("S");
            }

            //  eat redundant 'Z'
            if ((CharAt((m_current + 1)) == 'Z'))
            {
                m_current += 2;
            }
            else
            {
                m_current++;
            }

        }

        private bool Encode_ZZ()
        {
            //  "abruzzi", 'pizza' 
            if (((CharAt((m_current + 1)) == 'Z')
                        && ((StringAt((m_current + 2), 1, "I", "O", "A", "")
                        && ((m_current + 2)
                        == m_last))
                        || StringAt((m_current - 2), 9, "MOZZARELL", "PIZZICATO", "PUZZONLAN", ""))))
            {
                MetaphAdd("TS", "S");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Encode_ZU_ZIER_ZS()
        {
            if ((((m_current == 1)
                        && StringAt((m_current - 1), 4, "AZUR", ""))
                        || ((StringAt(m_current, 4, "ZIER", "")
                        && !StringAt((m_current - 2), 6, "VIZIER", ""))
                        || StringAt(m_current, 3, "ZSA", ""))))
            {
                MetaphAdd("J", "S");
                if (StringAt(m_current, 3, "ZSA", ""))
                {
                    m_current += 2;
                }
                else
                {
                    m_current++;
                }

                return true;
            }

            return false;
        }

        private bool Encode_French_EZ()
        {
            if ((((m_current == 3)
                        && StringAt((m_current - 3), 4, "CHEZ", ""))
                        || StringAt((m_current - 5), 6, "RENDEZ", "")))
            {
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_German_Z()
        {
            if (((m_current == 2) && ((m_current + 1) == m_last) && StringAt((m_current - 2), 4, "NAZI", ""))
                    || StringAt((m_current - 2), 6, "NAZIFY", "MOZART", "")
                    || StringAt((m_current - 3), 4, "HOLZ", "HERZ", "MERZ", "FITZ", "")
                    || (StringAt((m_current - 3), 4, "GANZ", "") && !IsVowel(m_current + 1))
                    || StringAt((m_current - 4), 5, "STOLZ", "PRINZ", "") || StringAt((m_current - 4), 7, "VENEZIA", "")
                    || StringAt((m_current - 3), 6, "HERZOG", "")
                    // german words beginning with "sch-" but not schlimazel, schmooze
                    || (m_inWord.Contains("SCH") && !(StringAt((m_last - 2), 3, "IZE", "OZE", "ZEL", "")))
                    || ((m_current > 0) && StringAt(m_current, 4, "ZEIT", ""))
                    || StringAt((m_current - 3), 4, "WEIZ", ""))
            {
                if ((m_current > 0) && m_inWord[m_current - 1] == 'T')
                {
                    MetaphAdd("S");
                }
                else
                {
                    MetaphAdd("TS");
                }
                m_current++;
                return true;
            }

            return false;
        }

        private bool Encode_ZH()
        {
            // Chinese pinyin e.g. 'zhao', also English "phonetic spelling"
            if ((CharAt((m_current + 1)) == 'H'))
            {
                MetaphAdd("J");
                m_current += 2;
                return true;
            }

            return false;
        }

        private bool Names_Beginning_With_SW_That_Get_Alt_SV()
        {
            if ((StringAt(0, 7, "SWANSON", "SWENSON", "SWINSON", "SWENSEN", "SWOBODA", "")
                        || (StringAt(0, 9, "SWIDERSKI", "SWARTHOUT", "") || StringAt(0, 10, "SWEARENGIN", ""))))
            {
                return true;
            }

            return false;
        }

        private bool Names_Beginning_With_SW_That_Get_Alt_XV()
        {
            if ((StringAt(0, 5, "SWART", "")
                        || (StringAt(0, 6, "SWARTZ", "SWARTS", "SWIGER", "")
                        || (StringAt(0, 7, "SWITZER", "SWANGER", "SWIGERT", "SWIGART", "SWIHART", "")
                        || (StringAt(0, 8, "SWEITZER", "SWATZELL", "SWINDLER", "")
                        || (StringAt(0, 9, "SWINEHART", "") || StringAt(0, 10, "SWEARINGEN", "")))))))
            {
                return true;
            }

            return false;
        }

        private bool Germanic_Or_Slavic_Name_Beginning_With_W()
        {
            if ((StringAt(0, 3, "WEE", "WIX", "WAX", "")
                        || (StringAt(0, 4, "WOLF", "WEIS", "WAHL", "WALZ", "WEIL", "WERT", "WINE", "WILK", "WALT", "WOLL", "WADA", "WULF", "WEHR", "WURM", "WYSE", "WENZ", "WIRT", "WOLK", "WEIN", "WYSS", "WASS", "WANN", "WINT", "WINK", "WILE", "WIKE", "WIER", "WELK", "WISE", "")
                        || (StringAt(0, 5, "WIRTH", "WIESE", "WITTE", "WENTZ", "WOLFF", "WENDT", "WERTZ", "WILKE", "WALTZ", "WEISE", "WOOLF", "WERTH", "WEESE", "WURTH", "WINES", "WARGO", "WIMER", "WISER", "WAGER", "WILLE", "WILDS", "WAGAR", "WERTS", "WITTY", "WIENS", "WIEBE", "WIRTZ", "WYMER", "WULFF", "WIBLE", "WINER", "WIEST", "WALKO", "WALLA", "WEBRE", "WEYER", "WYBLE", "WOMAC", "WILTZ", "WURST", "WOLAK", "WELKE", "WEDEL", "WEIST", "WYGAN", "WUEST", "WEISZ", "WALCK", "WEITZ", "WYDRA", "WANDA", "WILMA", "WEBER", "")
                        || (StringAt(0, 6, "WETZEL", "WEINER", "WENZEL", "WESTER", "WALLEN", "WENGER", "WALLIN", "WEILER", "WIMMER", "WEIMER", "WYRICK", "WEGNER", "WINNER", "WESSEL", "WILKIE", "WEIGEL", "WOJCIK", "WENDEL", "WITTER", "WIENER", "WEISER", "WEXLER", "WACKER", "WISNER", "WITMER", "WINKLE", "WELTER", "WIDMER", "WITTEN", "WINDLE", "WASHER", "WOLTER", "WILKEY", "WIDNER", "WARMAN", "WEYANT", "WEIBEL", "WANNER", "WILKEN", "WILTSE", "WARNKE", "WALSER", "WEIKEL", "WESNER", "WITZEL", "WROBEL", "WAGNON", "WINANS", "WENNER", "WOLKEN", "WILNER", "WYSONG", "WYCOFF", "WUNDER", "WINKEL", "WIDMAN", "WELSCH", "WEHNER", "WEIGLE", "WETTER", "WUNSCH", "WHITTY", "WAXMAN", "WILKER", "WILHAM", "WITTIG", "WITMAN", "WESTRA", "WEHRLE", "WASSER", "WILLER", "WEGMAN", "WARFEL", "WYNTER", "WERNER", "WAGNER", "WISSER", "")
                        || (StringAt(0, 7, "WISEMAN", "WINKLER", "WILHELM", "WELLMAN", "WAMPLER", "WACHTER", "WALTHER", "WYCKOFF", "WEIDNER", "WOZNIAK", "WEILAND", "WILFONG", "WIEGAND", "WILCHER", "WIELAND", "WILDMAN", "WALDMAN", "WORTMAN", "WYSOCKI", "WEIDMAN", "WITTMAN", "WIDENER", "WOLFSON", "WENDELL", "WEITZEL", "WILLMAN", "WALDRUP", "WALTMAN", "WALCZAK", "WEIGAND", "WESSELS", "WIDEMAN", "WOLTERS", "WIREMAN", "WILHOIT", "WEGENER", "WOTRING", "WINGERT", "WIESNER", "WAYMIRE", "WHETZEL", "WENTZEL", "WINEGAR", "WESTMAN", "WYNKOOP", "WALLICK", "WURSTER", "WINBUSH", "WILBERT", "WALLACH", "WYNKOOP", "WALLICK", "WURSTER", "WINBUSH", "WILBERT", "WALLACH", "WEISSER", "WEISNER", "WINDERS", "WILLMON", "WILLEMS", "WIERSMA", "WACHTEL", "WARNICK", "WEIDLER", "WALTRIP", "WHETSEL", "WHELESS", "WELCHER", "WALBORN", "WILLSEY", "WEINMAN", "WAGAMAN", "WOMMACK", "WINGLER", "WINKLES", "WIEDMAN", "WHITNER", "WOLFRAM", "WARLICK", "WEEDMAN", "WHISMAN", "WINLAND", "WEESNER", "WARTHEN", "WETZLER", "WENDLER", "WALLNER", "WOLBERT", "WITTMER", "WISHART", "WILLIAM", "")
                        || (StringAt(0, 8, "WESTPHAL", "WICKLUND", "WEISSMAN", "WESTLUND", "WOLFGANG", "WILLHITE", "WEISBERG", "WALRAVEN", "WOLFGRAM", "WILHOITE", "WECHSLER", "WENDLING", "WESTBERG", "WENDLAND", "WININGER", "WHISNANT", "WESTRICK", "WESTLING", "WESTBURY", "WEITZMAN", "WEHMEYER", "WEINMANN", "WISNESKI", "WHELCHEL", "WEISHAAR", "WAGGENER", "WALDROUP", "WESTHOFF", "WIEDEMAN", "WASINGER", "WINBORNE", "")
                        || (StringAt(0, 9, "WHISENANT", "WEINSTEIN", "WESTERMAN", "WASSERMAN", "WITKOWSKI", "WEINTRAUB", "WINKELMAN", "WINKFIELD", "WANAMAKER", "WIECZOREK", "WIECHMANN", "WOJTOWICZ", "WALKOWIAK", "WEINSTOCK", "WILLEFORD", "WARKENTIN", "WEISINGER", "WINKLEMAN", "WILHEMINA", "")
                        || (StringAt(0, 10, "WISNIEWSKI", "WUNDERLICH", "WHISENHUNT", "WEINBERGER", "WROBLEWSKI", "WAGUESPACK", "WEISGERBER", "WESTERVELT", "WESTERLUND", "WASILEWSKI", "WILDERMUTH", "WESTENDORF", "WESOLOWSKI", "WEINGARTEN", "WINEBARGER", "WESTERBERG", "WANNAMAKER", "WEISSINGER", "")
                        || (StringAt(0, 11, "WALDSCHMIDT", "WEINGARTNER", "WINEBRENNER", "")
                        || (StringAt(0, 12, "WOLFENBARGER", "") || StringAt(0, 13, "WOJCIECHOWSKI", ""))))))))))))
            {
                return true;
            }

            return false;
        }

        private bool Names_Beginning_With_J_That_Get_Alt_Y()
        {
            if ((StringAt(0, 3, "JAN", "JON", "JAN", "JIN", "JEN", "")
                        || (StringAt(0, 4, "JUHL", "JULY", "JOEL", "JOHN", "JOSH", "JUDE", "JUNE", "JONI", "JULI", "JENA", "JUNG", "JINA", "JANA", "JENI", "JOEL", "JANN", "JONA", "JENE", "JULE", "JANI", "JONG", "JOHN", "JEAN", "JUNG", "JONE", "JARA", "JUST", "JOST", "JAHN", "JACO", "JANG", "JUDE", "JONE", "")
                        || (StringAt(0, 5, "JOANN", "JANEY", "JANAE", "JOANA", "JUTTA", "JULEE", "JANAY", "JANEE", "JETTA", "JOHNA", "JOANE", "JAYNA", "JANES", "JONAS", "JONIE", "JUSTA", "JUNIE", "JUNKO", "JENAE", "JULIO", "JINNY", "JOHNS", "JACOB", "JETER", "JAFFE", "JESKE", "JANKE", "JAGER", "JANIK", "JANDA", "JOSHI", "JULES", "JANTZ", "JEANS", "JUDAH", "JANUS", "JENNY", "JENEE", "JONAH", "JONAS", "JACOB", "JOSUE", "JOSEF", "JULES", "JULIE", "JULIA", "JANIE", "JANIS", "JENNA", "JANNA", "JEANA", "JENNI", "JEANE", "JONNA", "")
                        || (StringAt(0, 6, "JORDAN", "JORDON", "JOSEPH", "JOSHUA", "JOSIAH", "JOSPEH", "JUDSON", "JULIAN", "JULIUS", "JUNIOR", "JUDITH", "JOESPH", "JOHNIE", "JOANNE", "JEANNE", "JOANNA", "JOSEFA", "JULIET", "JANNIE", "JANELL", "JASMIN", "JANINE", "JOHNNY", "JEANIE", "JEANNA", "JOHNNA", "JOELLE", "JOVITA", "JOSEPH", "JONNIE", "JANEEN", "JANINA", "JOANIE", "JAZMIN", "JOHNIE", "JANENE", "JOHNNY", "JONELL", "JENELL", "JANETT", "JANETH", "JENINE", "JOELLA", "JOEANN", "JULIAN", "JOHANA", "JENICE", "JANNET", "JANISE", "JULENE", "JOSHUA", "JANEAN", "JAIMEE", "JOETTE", "JANYCE", "JENEVA", "JORDAN", "JACOBS", "JENSEN", "JOSEPH", "JANSEN", "JORDON", "JULIAN", "JAEGER", "JACOBY", "JENSON", "JARMAN", "JOSLIN", "JESSEN", "JAHNKE", "JACOBO", "JULIEN", "JOSHUA", "JEPSON", "JULIUS", "JANSON", "JACOBI", "JUDSON", "JARBOE", "JOHSON", "JANZEN", "JETTON", "JUNKER", "JONSON", "JAROSZ", "JENNER", "JAGGER", "JASMIN", "JEPSEN", "JORDEN", "JANNEY", "JUHASZ", "JERGEN", "JAKOB", "")
                        || (StringAt(0, 7, "JOHNSON", "JOHNNIE", "JASMINE", "JEANNIE", "JOHANNA", "JANELLE", "JANETTE", "JULIANA", "JUSTINA", "JOSETTE", "JOELLEN", "JENELLE", "JULIETA", "JULIANN", "JULISSA", "JENETTE", "JANETTA", "JOSELYN", "JONELLE", "JESENIA", "JANESSA", "JAZMINE", "JEANENE", "JOANNIE", "JADWIGA", "JOLANDA", "JULIANE", "JANUARY", "JEANICE", "JANELLA", "JEANETT", "JENNINE", "JOHANNE", "JOHNSIE", "JANIECE", "JOHNSON", "JENNELL", "JAMISON", "JANSSEN", "JOHNSEN", "JARDINE", "JAGGERS", "JURGENS", "JOURDAN", "JULIANO", "JOSEPHS", "JHONSON", "JOZWIAK", "JANICKI", "JELINEK", "JANSSON", "JOACHIM", "JANELLE", "JACOBUS", "JENNING", "JANTZEN", "JOHNNIE", "")
                        || (StringAt(0, 8, "JOSEFINA", "JEANNINE", "JULIANNE", "JULIANNA", "JONATHAN", "JONATHON", "JEANETTE", "JANNETTE", "JEANETTA", "JOHNETTA", "JENNEFER", "JULIENNE", "JOSPHINE", "JEANELLE", "JOHNETTE", "JULIEANN", "JOSEFINE", "JULIETTA", "JOHNSTON", "JACOBSON", "JACOBSEN", "JOHANSEN", "JOHANSON", "JAWORSKI", "JENNETTE", "JELLISON", "JOHANNES", "JASINSKI", "JUERGENS", "JARNAGIN", "JEREMIAH", "JEPPESEN", "JARNIGAN", "JANOUSEK", "")
                        || (StringAt(0, 9, "JOHNATHAN", "JOHNATHON", "JORGENSEN", "JEANMARIE", "JOSEPHINA", "JEANNETTE", "JOSEPHINE", "JEANNETTA", "JORGENSON", "JANKOWSKI", "JOHNSTONE", "JABLONSKI", "JOSEPHSON", "JOHANNSEN", "JURGENSEN", "JIMMERSON", "JOHANSSON", "") || StringAt(0, 10, "JAKUBOWSKI", "")))))))))
            {
                return true;
            }

            return false;
        }

        void IStringMetaphoneEncoder.Encode(string source)
        {
            throw new System.NotImplementedException();
        }

        public bool Compare(string value1, string value2)
        {
            throw new System.NotImplementedException();
        }

        ////  example code
        //Metaphone3 m3 = new Metaphone3();
        //m3.SetEncodeVowels(true);
        //m3.SetEncodeExact(true);
        //m3.SetWord("iron");
        //m3.Encode();
        //System.out.println(("iron : " + m3.GetMetaph()));
        //System.out.println(("iron : (alt) " + m3.GetAlternateMetaph()));
        //m3.SetWord("witz");
        //m3.Encode();
        //System.out.println(("witz : " + m3.GetMetaph()));
        //System.out.println(("witz : (alt) " + m3.GetAlternateMetaph()));
        //m3.SetWord("");
        //m3.Encode();
        //System.out.println(("BLANK : " + m3.GetMetaph()));
        //System.out.println(("BLANK : (alt) " + m3.GetAlternateMetaph()));
        ////  these settings default to false
        //m3.SetEncodeExact(true);
        //m3.SetEncodeVowels(true);
        //string test = new string("Guillermo");
        //m3.SetWord(test);
        //m3.Encode();
        //System.out.println((test + (" : " + m3.GetMetaph())));
        //System.out.println((test + (" : (alt) " + m3.GetAlternateMetaph())));
        //test = "VILLASENOR";
        //m3.SetWord(test);
        //m3.Encode();
        //System.out.println((test + (" : " + m3.GetMetaph())));
        //System.out.println((test + (" : (alt) " + m3.GetAlternateMetaph())));
        //test = "GUILLERMINA";
        //m3.SetWord(test);
        //m3.Encode();
        //System.out.println((test + (" : " + m3.GetMetaph())));
        //System.out.println((test + (" : (alt) " + m3.GetAlternateMetaph())));
        //test = "PADILLA";
        //m3.SetWord(test);
        //m3.Encode();
        //System.out.println((test + (" : " + m3.GetMetaph())));
        //System.out.println((test + (" : (alt) " + m3.GetAlternateMetaph())));
        //test = "BJORK";
        //m3.SetWord(test);
        //m3.Encode();
        //System.out.println((test + (" : " + m3.GetMetaph())));
        //System.out.println((test + (" : (alt) " + m3.GetAlternateMetaph())));
        //test = "belle";
        //m3.SetWord(test);
        //m3.Encode();
        //System.out.println((test + (" : " + m3.GetMetaph())));
        //System.out.println((test + (" : (alt) " + m3.GetAlternateMetaph())));
        //test = "ERICH";
        //m3.SetWord(test);
        //m3.Encode();
        //System.out.println((test + (" : " + m3.GetMetaph())));
        //System.out.println((test + (" : (alt) " + m3.GetAlternateMetaph())));
        //test = "CROCE";
        //m3.SetWord(test);
        //m3.Encode();
        //System.out.println((test + (" : " + m3.GetMetaph())));
        //System.out.println((test + (" : (alt) " + m3.GetAlternateMetaph())));
        //test = "GLOWACKI";
        //m3.SetWord(test);
        //m3.Encode();
        //System.out.println((test + (" : " + m3.GetMetaph())));
        //System.out.println((test + (" : (alt) " + m3.GetAlternateMetaph())));
        //test = "qing";
        //m3.SetWord(test);
        //m3.Encode();
        //System.out.println((test + (" : " + m3.GetMetaph())));
        //System.out.println((test + (" : (alt) " + m3.GetAlternateMetaph())));
        //test = "tsing";
        //m3.SetWord(test);
        //m3.Encode();
        //System.out.println((test + (" : " + m3.GetMetaph())));
        //System.out.println((test + (" : (alt) " + m3.GetAlternateMetaph())));
        //}
    }

}
