using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontPreloader : MonoBehaviour
{
	private static readonly string kPrecacheFontGlyphsString= "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()-_+=~`[]{}|\\:;\"'<>,.?/ ";
	
	// utility struct used in font caching
	struct CacheFont
	{
		public Font font;
		public int fontSize;
		public FontStyle fontStyle;
	};

	void Awake() {
		DontDestroyOnLoad(this);
	}
	
	void OnLevelWasLoaded(int level)
	{
		// gather custom fonts that we will be using
		CacheFont[] myCustomFonts= RetrieveMyCustomFonts();
		
		if (null != myCustomFonts)
		{
			for (int fontIndex= 0; fontIndex < myCustomFonts.Length; ++fontIndex)
			{
				StartCoroutine(PrecacheFontGlyphs(
					myCustomFonts[fontIndex].font,
					myCustomFonts[fontIndex].fontSize,
					myCustomFonts[fontIndex].fontStyle,
					kPrecacheFontGlyphsString));
			}
		}
		
		return;
	}
	
	// Precache the font glyphs for the given font data.
	// Intended to run asynchronously inside of a coroutine.
	IEnumerator PrecacheFontGlyphs(Font font, int fontSize, FontStyle style, string glyphs)
	{
		Debug.Log("Precaching font \"" + font.name + "\", size \"" + fontSize + "\", style \"" + style + "\"");

		for (int index= 0; (index < glyphs.Length); ++index)
		{
			font.RequestCharactersInTexture(
				glyphs[index].ToString(),
				fontSize, style);
			yield return null;
		}

		Debug.Log(font.name + " precached.");
		
		yield break;
	}
	
	private CacheFont[] RetrieveMyCustomFonts() {
		Text[] textObjects = FindObjectsOfType<Text>() as Text[];
		List<CacheFont> cache = new List<CacheFont>();

		for (int i = 0; i < textObjects.Length; i++) {
			Text text = textObjects[i];
			CacheFont cacheFont = new CacheFont();

			cacheFont.font = text.font;
			cacheFont.fontSize = text.fontSize;
			cacheFont.fontStyle = text.fontStyle;

			bool alreadyInList = false;
			foreach (CacheFont listFont in cache) {
				if (cacheFont.font == listFont.font && cacheFont.fontSize == listFont.fontSize && cacheFont.fontStyle == listFont.fontStyle) {
					alreadyInList = true;
					break;
				}
			}

			if (!alreadyInList)
				cache.Add(cacheFont);
		}

		return cache.ToArray();
	}

};