using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml;
using System.Text.RegularExpressions;

[RequireComponent (typeof(Text))]
public class Quotes : MonoBehaviour {

    struct Quote {
        public string author;
        public string content;
    }
    
    public TextAsset quotesAsset;

    private List<Quote> quotes;
    private Text quoteText;
    private Text authorText;

    private string authorFormat;

    void Awake() {
        quoteText = GetComponent<Text>();
        authorText = transform.Find("Author").GetComponent<Text>();
        authorFormat = authorText.text;

        LoadQuotes();
    }

    private void LoadQuotes() {
        var doc = new XmlDocument();
        doc.LoadXml(quotesAsset.text);
        var quotesTags = doc.GetElementsByTagName("quote");

        quotes = new List<Quote>(quotesTags.Count);
        for (int i = 0; i < quotesTags.Count; i++) {
            var node = quotesTags.Item(i);

            Quote q = new Quote();
            q.author = GetAuthorOrNull(node);
            q.content = GetContent(node);
            quotes.Add(q);
        }
    }

    private string GetAuthorOrNull(XmlNode quoteNode) {
        var authorNode = quoteNode.Attributes.GetNamedItem("author");
        if (authorNode != null)
            return authorNode.Value;
        return null;
    }

    private string GetContent(XmlNode quoteNode) {
        string content = quoteNode.InnerText.Trim();
        content = Regex.Replace(content, @"\s+", " ");
        content = content.Replace("\\n ", "\n");
        return content;
    }

    void Start() {
        Change();
    }

	public void Change() {
        int i = Random.Range(0, quotes.Count);
        Quote quote = quotes[i];

        quoteText.text = quote.content;
        authorText.text = string.Format(authorFormat, quote.author);
        authorText.enabled = quote.author != null;
    }
}
