using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DontTrust.Characters.Main
{
	public class MainCharacterAudio : MonoBehaviour {

		[SerializeField] private AudioClip m_DamageSound;
		[SerializeField] private AudioClip m_LandingSound;

		private AudioSource m_AudioSrc;

		// Use this for initialization
		void Start () {
			m_AudioSrc = GetComponent<AudioSource> ();
		}
		
		public void PlayDamageSound()
		{
			m_AudioSrc.PlayOneShot (m_DamageSound);
		}

		public void PlayLandingSound()
		{
			m_AudioSrc.PlayOneShot (m_LandingSound, 0.5f);
		}
	}
}