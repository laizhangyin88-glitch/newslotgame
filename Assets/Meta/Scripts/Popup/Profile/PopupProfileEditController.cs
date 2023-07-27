using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;
using UnityEngine.UI;
using SlotMaker;
using BagelCode.ClientModels;

namespace BagelCode
{
    public class PopupProfileEditController : MonoBehaviour
    {
        private Blackboard rootBB;
        private Animator rootAnimator;
        private ContextElement rootElement;

        private ContextElement nameInputElement;
        private ContextElement commentInputElement;
        private ContextElement genderDropDownElement;
        private ContextElement ageInputElement;
        private ContextElement countryIconAreaElement;
        private ContextElement countryButtonElement;
        private ContextElement countryNameTextElement;
        private ContextElement countryIconElement;

        private ContextElement profileAreaElement;
        private ContextElement profileImageElement;

        private ContextElement saveButtonElement;
        private ContextElement changePhotoButtonElement;
        private ContextElement closeButtonElement;

        private Blackboard profileBB;

        private StringTable.StringTableType tableType = StringTable.StringTableType.Global;

        private const string ON_SAVE = "OnSave";
        private const string ON_CHANGE_PHOTO = "OnChangePhoto";
        private const string ON_OPEN_COUNTRY_SELECT = "OnOpenCountrySelect";
        private const string ON_CLOSE = "OnClose";

        private List<string> genderDropDownOptions = new List<string>()
            {
                "<align=left><pos=33>Blank",
                "<align=left><sprite name=M>Male",
                "<align=left><sprite name=F>Female",
                "<align=left><sprite name=O>Other"
            };

        private bool isInit = false;

        private string prevName;
        private int prevAge;

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void InitProperty()
        {
            if(isInit) return;

            rootAnimator = gameObject.GetComponent<Animator>();
            rootBB = gameObject.GetComponent<Blackboard>();
            rootElement = gameObject.GetComponent<ContextElement>();
            rootElement.UpdateContext(false);

            MetaContextElementUtils.SimpleSetText(rootElement, "Title Area/Text", StringTableUtils.GetString(tableType, "POPUP_PROFILE_EDIT_TITLE"), ContextSearchingType.FullNameSearch);

            nameInputElement = ContextUtils.FindElement(rootElement, "Name/Input Field", ContextSearchingType.FullNameSearch);
            commentInputElement = ContextUtils.FindElement(rootElement, "Comment/Input Field", ContextSearchingType.FullNameSearch);
            genderDropDownElement = ContextUtils.FindElement(rootElement, "Gender/Dropdown", ContextSearchingType.FullNameSearch);
            ageInputElement = ContextUtils.FindElement(rootElement, "Age/Input Field", ContextSearchingType.FullNameSearch);

            countryButtonElement = ContextUtils.FindElement(rootElement, "Country/Button Country", ContextSearchingType.FullNameSearch);

            countryIconAreaElement = ContextUtils.FindElement(countryButtonElement, "Icon Country Area", ContextSearchingType.ChildrenSearch);
            var imageIconObj = MetaObjectUtils.MakePrefab("Icon Image", countryIconAreaElement.transform);
            countryIconElement = imageIconObj.GetComponent<ContextElement>();
            countryNameTextElement = ContextUtils.FindElement(countryButtonElement, "Text", ContextSearchingType.ChildrenSearch);

            profileAreaElement = ContextUtils.FindElement(rootElement, "Profile Picture", ContextSearchingType.ChildrenSearch);
            profileImageElement = ContextUtils.FindElement(profileAreaElement, "Image", ContextSearchingType.ChildrenSearch);

            saveButtonElement = ContextUtils.FindElement(rootElement, "Button Save", ContextSearchingType.ChildrenSearch);
            changePhotoButtonElement = ContextUtils.FindElement(rootElement, "Button Change Photo", ContextSearchingType.ChildrenSearch);
            closeButtonElement = ContextUtils.FindElement(rootElement, "Button Close", ContextSearchingType.ChildrenSearch);

            MetaContextElementUtils.SimpleSetActive(rootElement, "Profile Edit Gain Vip Point", false, ContextSearchingType.ChildrenSearch);

            profileBB = profileAreaElement.gameObject.GetComponent<Blackboard>();

            MetaContextElementUtils.SimpleSetText(saveButtonElement, "Text", StringTableUtils.GetString(tableType, "BUTTON_SAVE"));
            MetaContextElementUtils.SetClickable(
                saveButtonElement,
                ON_SAVE,
                rootElement,
                null
            );

            MetaContextElementUtils.SimpleSetText(changePhotoButtonElement, "Text", StringTableUtils.GetString(tableType, "BUTTON_CHANGE_PHOTO"));
            MetaContextElementUtils.SetClickable(
                changePhotoButtonElement,
                ON_CHANGE_PHOTO,
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                countryButtonElement,
                ON_OPEN_COUNTRY_SELECT,
                rootElement,
                null
            );

            MetaContextElementUtils.SetClickable(
                closeButtonElement,
                ON_CLOSE,
                rootElement,
                null
            );

            MetaContextElementUtils.SetInputFieldAttrribute(nameInputElement, 20, InputField.ContentType.Standard);
            MetaContextElementUtils.SetInputFieldAttrribute(commentInputElement, 30, InputField.ContentType.Standard);
            MetaContextElementUtils.SetInputFieldAttrribute(ageInputElement, 3, InputField.ContentType.IntegerNumber);

            MetaContextElementUtils.SetDropdownStringOptions(genderDropDownElement, genderDropDownOptions);

            isInit = true;
        }

        public void UpdateProfileValues()
        {
            InitProperty();

            var userInfoBB      = BlackboardUtils.GetOrCreateVariable<Blackboard>(MainBlackboard.Get(), "me");
            string name         = StringTable.BadWordFilter(userInfoBB.value.GetValue<string>("name"));
            string comment      = StringTable.BadWordFilter(userInfoBB.value.GetValue<string>("message"));
            Gender gender       = userInfoBB.value.GetValue<Gender>("gender");
            int age             = userInfoBB.value.GetValue<int>("age");
            string countryCode  = userInfoBB.value.GetValue<string>("countrySelected");
            string profileURL   = userInfoBB.value.GetValue<string>("profileUrl");
            int tier            = userInfoBB.value.GetValue<int>("tier");
            int tierGroup       = TierUtils.GetTierGroup(tier);

            MetaContextElementUtils.SetText(nameInputElement, name);
            MetaContextElementUtils.SetText(commentInputElement, comment);
            MetaContextElementUtils.SetText(ageInputElement, age.ToString());
            MetaContextElementUtils.SetIntProperty(genderDropDownElement, (int)gender);

            MetaContextElementUtils.SetWebImage(profileImageElement, profileURL, CacheType.MemCache, false, null);
            BlackboardUtils.SetOrCreateValue<int>(profileBB, "tierGroup", tierGroup);

            prevName = name;
            prevAge = age;

            UpdateCountry(countryCode);
        }

        public void UpdateCountry(string countryCode)
        {
            BlackboardUtils.SetOrCreateValue<string>(rootBB, "countryCode", countryCode);

            if(CountryUtils.ExistCountryCode(countryCode))
            {
                string countryName = StringTableUtils.GetString(tableType, "POPUP_PROFILE_EDIT_COUNTRY_TEXT", CountryUtils.GetCountryName(countryCode) );
                MetaContextElementUtils.SetText(countryNameTextElement, countryName);

                MetaContextElementUtils.SetContextCountryImage(countryIconElement, countryCode);
                countryIconElement.gameObject.SetActive(true);
            }
            else
            {
                MetaContextElementUtils.SetText(countryNameTextElement, "-");
                countryIconElement.gameObject.SetActive(false);
            }
        }

        public void UpdateEditInfo()
        {
            var name = MetaContextElementUtils.GetText(nameInputElement);
            if(string.IsNullOrEmpty(name))
                name = prevName;

            BlackboardUtils.SetOrCreateValue<string>(rootBB, "name", name);

            var age = MetaContextElementUtils.GetText(ageInputElement);
            if(string.IsNullOrEmpty(age))
                age = prevAge.ToString();

            try
            {
                BlackboardUtils.SetOrCreateValue<int>(rootBB, "age", System.Convert.ToInt32(age));
            }
            catch (System.FormatException e)
            {
                BlackboardUtils.SetOrCreateValue<int>(rootBB, "age", prevAge);
            }
            // BlackboardUtils.SetOrCreateValue<int>(rootBB, "age", System.Convert.ToInt32(MetaContextElementUtils.GetText(ageInputElement)));

            BlackboardUtils.SetOrCreateValue<string>(rootBB, "comment", MetaContextElementUtils.GetText(commentInputElement));
            BlackboardUtils.SetOrCreateValue<Gender>(rootBB, "gender", (Gender)MetaContextElementUtils.GetIntProperty(genderDropDownElement));
        }
    }
}
