using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using Rnd = UnityEngine.Random;

public class wordDisassociationModule : MonoBehaviour {
	[HideInInspector] public KMBombInfo Bomb;
    [HideInInspector] public KMAudio Audio;
	[HideInInspector] public KMBombModule Module;

	[SerializeField] private KMSelectable[] _letters;
	[SerializeField] private KMSelectable _backspace;
	[SerializeField] private KMSelectable _submit;
	[SerializeField] private TextMesh _displayText;

	private static int _modIdCounter = 1;
	private int _modId;
	private bool IsSolved = false;
	private readonly string [][] _lyrics = new string[5][] { 
    	new string[] { "Enemy","lasagna","Robust","below","wax","Semiautomatic","aqua","Accompany","slacks","Why","coffee","gymnastic","Motorcycle","unibrow","Existential","plastic","extra","nightly","cow","Damn","jettison","goodbye","through","Everything","center","who","Spidery","concubine","Pale","lickitysplit","remorse","Vitamin","after","force","Already","nested","human","wine" },
    	new string[] { "Flight","Luminary","uprise","Entanglement","broke","Unsophisticated","clockwise","Holiday","way","smoke","Abundant","various","Metaphorically","applause","Underneath","hilarious","oxymoron","claws","Rectangular","awkward","hurt","Million","controvert","Never","undressing","sneer","Blue","therapy","fall","inside","Father","dethrone","applied","Guillotine","apprehensive","engineer" },
		new string[] { "Prance","omelette","stalking","chimneysweep","Eleven","hatred","earmuff","okay","rathskeller","My","elusive","hula","yellow","sketching","creamy","helium","gentlemanly","communique" },
		new string[] { "Flouncy","Panicky","redundant","Psychedelic","while","Raisin","terrible","abundant","Polyurethane","smile","Scrumptious","mechanical","Jungle","uncle","wish","Paleobotanical","backwards","licorice","Truth","medical","entertain","Cleverly","porridge","brain","Jellyfish","fingernail","Agnostic","oppressive","wall","Platypus","parasol","Sauntering","sawdust","opera","monorail" },
		new string[] { "Letter","no","sly","violin","dustbunny","Explode","serenade","why","spoil","play","drip","Skullduggery","freezer","monocle","pelican","Cool","milk","freak","tongue","television","staplegun","Mellow","face","bubblegum","periscope","fight","silly","Elephant","akimbo","paranoia","sever","maybe","Crush","toy","spoon","melt","feather","clear","king","weird","Space","love","domino","reality","apostrophe","Dollar","jade","velocity","meringue","assuming","gentle","mister","Advertisement","suitcase","pining","lobsters","over","murderous","Distraction","flames","imposter","acapella","crouch","about","bionic","Ruby","quickly","antidisestablishmentarianism" }
    };
	private string _solution;
	private readonly char[] _vowels = { 'a','e','i','o','u','y' };
	private readonly char[] _consonants = { 'b','c','d','f','g','h','j','k','l','m','n','p','q','r','s','t','v','w','x','z' };
	private void Awake() { 

		_modId = _modIdCounter++;
		
		Bomb = GetComponent<KMBombInfo>();
        Audio = GetComponent<KMAudio>();
        Module = GetComponent<KMBombModule>();

		foreach (KMSelectable letter in _letters) {
			letter.OnInteract += delegate() { handleLetterPress(letter.name); return false; };
		}
		_backspace.OnInteract += delegate() { handleBackspacePress(); return false; };
		_submit.OnInteract += delegate() { handleSubmit(); return false; };
		
	}
	private void handleLetterPress(string letter) { 
		if (IsSolved) { 
			return; 
		}
		_displayText.text += letter;
	}
	private void handleBackspacePress() {
		if (_displayText.text != "") {
			_displayText.text = _displayText.text.Remove(_displayText.text.Length - 1);
		}
	}
	private void handleSubmit() {
		if (_displayText.text == _solution) { Module.HandlePass(); }
		else { Module.HandleStrike(); }
	}
	//Debug.Log($"[wordDisassociationModule {_modId}] Ahoy!") 

	// Use this for initialization
	void Start () {
		int firstDigit = Bomb.GetSerialNumberNumbers().ElementAt(0) % 5;
		_solution = _lyrics[firstDigit][Rnd.Range(0, _lyrics[firstDigit].Length)].ToUpper();
		Debug.Log(_solution);
	}
	private bool[5] WordAssociation(string first, string second) {
		char[] firstArray = first.ToCharArray(); 
		char[] secondArray = second.ToCharArray(); 

		bool consonantRule = FilterString(first, _consonants) == FilterString(second, _consonants) ? true : false;
		return { }
	}
	private bool ConsonantRule(string first, string second) {
		if (FilterString(first, _consonants) == FilterString(second, _consonants)) { return true; }
		return false;
	}
	private bool VowelRule(string first, string second) {
		if (FilterString(first, _consonants) == FilterString(second, _consonants)) { return true; }
		return false;
	}
	private bool FirstLastRule(string first, string second) {
		if ((first.ToCharArray()[0] == second.ToCharArray()[0]) && (first.ToCharArray()[first.Length] == second.ToCharArray()[second.Length])) { return true; }
		return false;
	}
	private bool SubstringRule(string first, string second) {
		char[] firstArray = first.ToCharArray();
		char[] secondArray = second.ToCharArray();
		return false;
	}
	private string FilterString(string word, char[] desiredChars) {
		char[] letters = word.ToCharArray();
		string filteredString = (string)letters.Where(letter => desiredChars.Contains(letter));
		return filteredString;
	}
	
}	

