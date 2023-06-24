using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class wordDisassociationModule : MonoBehaviour {
	[SerializeField] private KMBombInfo Bomb;
    [SerializeField] private KMAudio Audio;
	[SerializeField] private KMBombModule Module;

	[SerializeField] private KMSelectable[] _letters;
	[SerializeField] private KMSelectable _backspace;
	[SerializeField] private KMSelectable _submit;
	[SerializeField] private TextMesh _displayText;

	private static int _modIdCounter = 1;
	private int _modId;
	private bool _isSolved = false;
	private readonly string [][] _lyrics = new string[5][] { 
    	new string[] { "ENEMY","LASAGNA","ROBUST","BELOW","WAX","SEMIAUTOMATIC","AQUA","ACCOMPANY","SLACKS","WHY","COFFEE","GYMNASTIC","MOTORCYCLE","UNIBROW","EXISTENTIAL","PLASTIC","EXTRA","NIGHTLY","COW","DAMN","JETTISON","GOODBYE","THROUGH","EVERYTHING","CENTER","WHO","SPIDERY","CONCUBINE","PALE","LICKITYSPLIT","REMORSE","VITAMIN","AFTER","FORCE","ALREADY","NESTED","HUMAN","WINE" },
    	new string[] { "FLIGHT","LUMINARY","UPRISE","ENTANGLEMENT","BROKE","UNSOPHISTICATED","CLOCKWISE","HOLIDAY","WAY","SMOKE","ABUNDANT","VARIOUS","METAPHORICALLY","APPLAUSE","UNDERNEATH","HILARIOUS","OXYMORON","CLAWS","RECTANGULAR","AWKWARD","HURT","MILLION","CONTROVERT","NEVER","UNDRESSING","SNEER","BLUE","THERAPY","FALL","INSIDE","FATHER","DETHRONE","APPLIED","GUILLOTINE","APPREHENSIVE","ENGINEER" },
		new string[] { "PRANCE","OMELETTE","STALKING","CHIMNEYSWEEP","ELEVEN","HATRED","EARMUFF","OKAY","RATHSKELLER","MY","ELUSIVE","HULA","YELLOW","SKETCHING","CREAMY","HELIUM","GENTLEMANLY","COMMUNIQUE" },
		new string[] { "FLOUNCY","PANICKY","REDUNDANT","PSYCHEDELIC","WHILE","RAISIN","TERRIBLE","ABUNDANT","POLYURETHANE","SMILE","SCRUMPTIOUS","MECHANICAL","JUNGLE","UNCLE","WISH","PALEOBOTANICAL","BACKWARDS","LICORICE","TRUTH","MEDICAL","ENTERTAIN","CLEVERLY","PORRIDGE","BRAIN","JELLYFISH","FINGERNAIL","AGNOSTIC","OPPRESSIVE","WALL","PLATYPUS","PARASOL","SAUNTERING","SAWDUST","OPERA","MONORAIL" },
		new string[] { "LETTER","NO","SLY","VIOLIN","DUSTBUNNY","EXPLODE","SERENADE","WHY","SPOIL","PLAY","DRIP","SKULLDUGGERY","FREEZER","MONOCLE","PELICAN","COOL","MILK","FREAK","TONGUE","TELEVISION","STAPLEGUN","MELLOW","FACE","BUBBLEGUM","PERISCOPE","FIGHT","SILLY","ELEPHANT","AKIMBO","PARANOIA","SEVER","MAYBE","CRUSH","TOY","SPOON","MELT","FEATHER","CLEAR","KING","WEIRD","SPACE","LOVE","DOMINO","REALITY","APOSTROPHE","DOLLAR","JADE","VELOCITY","MERINGUE","ASSUMING","GENTLE","MISTER","ADVERTISEMENT","SUITCASE","PINING","LOBSTERS","OVER","MURDEROUS","DISTRACTION","FLAMES","IMPOSTER","ACAPELLA","CROUCH","ABOUT","BIONIC","RUBY","QUICKLY","ANTIDISESTABLISHMENTARIANISM" }
    };
	private string _generatedSolution;
	private string[] _possibleSolutions;
	private readonly char[] _vowels = { 'a','e','i','o','u','y' };
	private readonly char[] _consonants = { 'b','c','d','f','g','h','j','k','l','m','n','p','q','r','s','t','v','w','x','z' };

	private void Awake() { 

		_modId = _modIdCounter++;

		foreach (KMSelectable letter in _letters) {
			letter.OnInteract += delegate() { handleLetterPress(letter.name); return false; };
		}
		_backspace.OnInteract += delegate() { handleBackspacePress(); return false; };
		_submit.OnInteract += delegate() { handleSubmitPress(); return false; };

	}
	private void handleLetterPress(string letter) { 
		if (_isSolved) { return; }
		_displayText.text += letter;
	}
	private void handleBackspacePress() {
		if (_isSolved) { return; }
		if (_displayText.text != "") {
			_displayText.text = _displayText.text.Remove(_displayText.text.Length - 1);
		}
	}
	private void handleSubmitPress() {
		if (_isSolved) { return; }
		if (_possibleSolutions.Contains(_displayText.text)) { Solve(""); }
		else { Strike($"Submitted {_displayText.text} when expecting {_generatedSolution}."); }
	}
	private void Solve(string message) {
		Log(message);
		Module.HandlePass();
		_isSolved = true;
	}
	private void Strike(string reason) {
		Log(new string[] { "Incorrect answer! Issuing strike.", reason });
		Module.HandleStrike();
	}
	private void Log(string message) {
		Debug.Log($"[wordDisassociationModule #{_modId}] {message}");
	}
	private void Log(string[] message) {
		foreach(string line in message) {
			Debug.Log($"[wordDisassociationModule #{_modId}] {line}");
		}
	}
	void Start () {
		int i = Bomb.GetSerialNumberNumbers().ElementAt(0) % 5;
		_generatedSolution = _lyrics[i][Rnd.Range(0, _lyrics[i].Length)].ToUpper();
		_possibleSolutions = new string[] { _generatedSolution, "BRUH" };
		Log($"Possible solutions: {_possibleSolutions.Join(" ")}");
	}
 	private bool[] WordAssociation(string first, string second) {
		bool[] Associations = { ConsonantRule(first, second), VowelRule(first, second), FirstLastRule(first, second), SubstringRule(first, second), VowelConsonantPatternRule(first, second) };
		return Associations;
	}
	private bool ConsonantRule(string first, string second) {
		if (FilterString(first, _consonants) == FilterString(second, _consonants)) { return true; }
		return false;
	}
	private bool VowelRule(string first, string second) {
		if (FilterString(first, _vowels) == FilterString(second, _vowels)) { return true; }
		return false;
	}
	private bool FirstLastRule(string first, string second) {
		if ((first[0] == second[0]) && (first[first.Length] == second[second.Length])) { return true; }
		return false;
	}
	private bool SubstringRule(string first, string second) {
		for (int i = 0; i < first.Length - 2; i++) {
			for (int j = 0; j < second.Length - 2; j++) {
				if (first.Substring(i, 3) == second.Substring(j, 3)) { 
					return true;
				}
			}
		}
		return false;
	}
	private bool VowelConsonantPatternRule(string first, string second) {
		if (first.Length == second.Length) {
			for (int i = 0; i < first.Length; i++) {
				if (!(_vowels.Contains(first[i]) == _vowels.Contains(second[i]))) {
					return false;
				}
			}
			return true;
		}
		return false;
	}
	private string FilterString(string word, char[] desiredChars) {
		char[] letters = word.ToCharArray();
		string filteredString = letters.Where(letter => desiredChars.Contains(letter)).ToString();
		return filteredString;
	}

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} to do something.x";
#pragma warning restore 414

    // Twitch Plays (TP) documentation: https://github.com/samfundev/KtaneTwitchPlays/wiki/External-Mod-Module-Support

    IEnumerator ProcessTwitchCommand (string Command) {
        yield return null;
    }

    IEnumerator TwitchHandleForcedSolve () {
        yield return null;
    }

	/* basic support
    KMSelectable[] ProcessTwitchCommand (string Command) {
        return null;
    }

    KMSelectable[] TwitchHandleForcedSolve () {
        return null;
    }*/
}	