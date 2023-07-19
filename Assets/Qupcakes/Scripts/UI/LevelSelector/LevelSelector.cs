using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Reference: https://www.google.com/search?q=unity+create+level+selection+menu&rlz=1C5CHFA_en__708__708&oq=unity+create+level+selection+menu&aqs=chrome..69i57j33i22i29i30.8223j0j9&sourceid=chrome&ie=UTF-8#kpvalbx=_WhAMYfrTB6Gvggf1y5OYCQ56

namespace Qupcakery
{
    public class LevelSelector : MonoBehaviour
    {
        public GameObject levelHolder;
        public GameObject levelIcon;
        public GameObject thisCanvas;
        public GameObject litePanel;
        public int numberOfLevels;

        public Sprite[] levelNumbers = new Sprite[27];

        Rect panelDimensions, iconDimensions;
        int amountPerPage;
        int currentLevelCount = 0;

        // Start is called before the first frame update
        void Start()
        {
            if (!TutorialManager.IntroPlayed)
            {
                Wrapper.Events.StartDialogueSequence?.Invoke("QU_Start");
                TutorialManager.IntroPlayed = true;
            }

            numberOfLevels = GameManagement.Instance.GetTotalLevelCnt();

#if LITE_VERSION
    litePanel.SetActive(true);
#endif

            panelDimensions = levelHolder.GetComponent<RectTransform>().rect;
            iconDimensions = levelIcon.GetComponent<RectTransform>().rect;
            int maxInARow = Mathf.FloorToInt(panelDimensions.width / iconDimensions.width);
            int maxInACol = Mathf.FloorToInt(panelDimensions.height / iconDimensions.height);
            amountPerPage = maxInARow * maxInACol;
            int totalPages = Mathf.CeilToInt((float)numberOfLevels / amountPerPage);
            LoadPanels(totalPages);
        }

        void LoadPanels(int numberOfPanels)
        {
            GameObject panelClone = Instantiate(levelHolder) as GameObject;
            PageSwiper swiper = levelHolder.AddComponent<PageSwiper>();
            swiper.totalPages = numberOfPanels;

            for (int i = 1; i <= numberOfPanels; i++)
            {
                GameObject panel = Instantiate(panelClone) as GameObject;
                panel.transform.SetParent(thisCanvas.transform, false); // Ensures the child panel has the same dimensions as the parent
                panel.transform.SetParent(levelHolder.transform);
                panel.name = "Page-" + i;
                panel.GetComponent<RectTransform>().localPosition = new Vector2(panelDimensions.width * (i - 1), 0);
                SetupGrid(panel);
                int numberOfIcons = (i == numberOfPanels) ? numberOfLevels - currentLevelCount : amountPerPage;
                LoadIcons(numberOfIcons, panel);
            }

            Destroy(panelClone);
        }

        void SetupGrid(GameObject panel)
        {
            GridLayoutGroup grid = panel.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(iconDimensions.width, iconDimensions.height);
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 9;
        }

        void LoadIcons(int numberOfIcons, GameObject parentObject)
        {
            for (int i = 0; i < numberOfIcons; i++)
            {
                currentLevelCount++;
                GameObject icon = Instantiate(levelIcon) as GameObject;
                icon.transform.SetParent(thisCanvas.transform, false);
                icon.transform.SetParent(parentObject.transform);
                icon.name = "Level" + currentLevelCount;
                LevelSelectorButtonManager m =
                    icon.GetComponent<LevelSelectorButtonManager>();
                m.SetMode(GameManagement.Instance.gameMode);

                // If player has completed this level
                if (currentLevelCount <= GameManagement.Instance.game.gameStat.MaxLevelCompleted)
                {
                    int starCnt = GameManagement.Instance.game.gameStat.GetLevelPerformance(currentLevelCount);
                    //Debug.Log("player has completed level " + currentLevelCount);
                    //Debug.Log("max level completed " + GameManagement.Instance.game.gameStat.MaxLevelCompleted);
                    m.SetStar(starCnt);
                } else // If player has not completed this level
                {
                    if (currentLevelCount != 1 + GameManagement.Instance.game.gameStat.MaxLevelCompleted)
                        m.SetAvailability(false);
                }
                icon.GetComponentInChildren<TextMeshProUGUI>().SetText("Level " + currentLevelCount);

                // comment this part out if you want the originial level select icon
                Image levelNumberObj = icon.transform.Find("Number").GetComponent<Image>();
                levelNumberObj.sprite = levelNumbers[currentLevelCount-1];
            }

        }
    }
}
