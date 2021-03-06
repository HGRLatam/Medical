﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical.Controller;
using MyGUIPlugin;
using System.Reflection;

namespace Medical
{
    public enum AnatomyPickingMode
    {
        Group,
        Individual,
        None,
    }

    public enum SelectionOperator
    {
        Select,
        Add,
        Remove
    }

    public enum SuggestedDisplaySortMode
    {
        None,
        Alphabetical
    }

    public class AnatomyController : IDisposable
    {
        private AnatomyLuceneSearch luceneSearch;

        private AnatomyPickingMode pickingMode = AnatomyPickingMode.Group;
        private SelectionOperator selectionOperator = SelectionOperator.Select;
        private bool showPremiumAnatomy = true;
        private AnatomyCommandPermissions commandPermissions = AnatomyCommandPermissions.None;
        private AnatomyFilterEntry currentTopLevelMode = null;
        private AnatomySelection selectedAnatomy = new AnatomySelection();

        public event EventDelegate<AnatomyController> AnatomyChanged;
        public event EventDelegate<AnatomyController, AnatomyPickingMode> PickingModeChanged;
        public event EventDelegate<AnatomyController, bool> ShowPremiumAnatomyChanged;
        public event EventDelegate<AnatomyController, AnatomyCommandPermissions> CommandPermissionsChanged;
        public event EventDelegate<AnatomyController, SelectionOperator> SelectionOperatorChanged;

        /// <summary>
        /// Called when a piece of anatomy has been searched for and should be displayed.
        /// </summary>
        public event Action<Anatomy> DisplayAnatomy;

        /// <summary>
        /// Called when the display of anatomy should be cleared.
        /// </summary>
        public event Action ClearDisplayedAnatomy;

        /// <summary>
        /// Fired when a search is started. This fires for all types of searches.
        /// </summary>
        public event Action<SuggestedDisplaySortMode> SearchStarted;

        /// <summary>
        /// Fired when a search is started that needs to suggest a caption.
        /// </summary>
        public event Action<String> SuggestSearchCaption;

        /// <summary>
        /// Fired when a search is ended. This fires for all types of searches.
        /// </summary>
        public event Action SearchEnded;

        public AnatomyController()
        {
            luceneSearch = new AnatomyLuceneSearch(this);
            currentTopLevelMode = luceneSearch.FilterEntries.First();
            selectedAnatomy.DisplaySelectedAnatomy += selectedAnatomy_DisplaySelectedAnatomy;
        }

        public void Dispose()
        {
            luceneSearch.Dispose();
        }

        public void sceneLoaded()
        {
            luceneSearch.setAnatomy(AnatomyManager.AnatomyList, AnatomyManager.AnatomyOrganizer);
            if (AnatomyChanged != null)
            {
                AnatomyChanged.Invoke(this);
            }
        }

        public void sceneUnloading()
        {
            luceneSearch.clear();
        }

        /// <summary>
        /// Find the anatomy along a given ray. Will fire search events. Returns the best match anatomy based
        /// on position and selection mode (group or individual). Returns null if no anatomy was found
        /// along the ray or if the picking mode is null. If a value is returned it will have at least one result.
        /// </summary>
        /// <param name="ray">The ray to check for anatomy along.</param>
        /// <returns>An enumerator over results or null if there are no results to enumerate.</returns>
        public IEnumerable<Anatomy> findAnatomy(Ray3 ray, out AnatomyIdentifier firstMatch)
        {
            firstMatch = null;

            fireSearchStarted(SuggestedDisplaySortMode.Alphabetical);
            fireClearDisplayedAnatomy();

            IEnumerable<Anatomy> results = null;

            var matches = AnatomyManager.findAnatomy(ray);

            if (matches.Count > 0)
            {
                //Display found anatomy and related groups
                HashSet<String> displayedGroups = new HashSet<String>();
                foreach (AnatomyIdentifier anatomy in matches.Anatomy)
                {
                    if(firstMatch == null)
                    {
                        firstMatch = anatomy;
                    }

                    fireDisplayAnatomy(anatomy);
                    foreach (var group in luceneSearch.relatedGroupsFor(anatomy))
                    {
                        if (group.ShowInClickSearch && displayedGroups.Add(group.AnatomicalName))
                        {
                            fireDisplayAnatomy(group);
                        }
                    }
                }

                if (pickingMode != AnatomyPickingMode.None)
                {
                    results = currentClickGroupSelectionFor(matches);
                }
            }
            else
            {
                foreach (var anatomy in currentTopLevelMode.TopLevelItems)
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            fireSearchEnded();

            return results;
        }

        public void findAnatomy(String searchTerm, IEnumerable<AnatomyFacet> facets)
        {
            if (String.IsNullOrEmpty(searchTerm))
            {
                fireSearchStarted(SuggestedDisplaySortMode.Alphabetical);
                fireClearDisplayedAnatomy();

                foreach (Anatomy anatomy in currentTopLevelMode.TopLevelItems)
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            else
            {
                fireSearchStarted(SuggestedDisplaySortMode.None);
                fireClearDisplayedAnatomy();

                foreach(var anatomy in luceneSearch.search(searchTerm, facets, 35))
                {
                    fireDisplayAnatomy(anatomy);
                }
            }
            fireSearchEnded();
        }

        public void displayAnatomy(String searchCaption, IEnumerable<Anatomy> anatomyToDisplay, SuggestedDisplaySortMode sortMode)
        {
            fireSuggestSearchCaption(searchCaption);
            fireSearchStarted(sortMode);
            fireClearDisplayedAnatomy();
            foreach (Anatomy relatedAnatomy in anatomyToDisplay)
            {
                fireDisplayAnatomy(relatedAnatomy);
            }
            fireSearchEnded();
        }

        public AnatomyPickingMode PickingMode
        {
            get
            {
                return pickingMode;
            }
            set
            {
                if (value != pickingMode)
                {
                    pickingMode = value;
                    if (PickingModeChanged != null)
                    {
                        PickingModeChanged.Invoke(this, value);
                    }
                }
            }
        }

        public SelectionOperator SelectionOperator
        {
            get
            {
                return selectionOperator;
            }
            set
            {
                if(value != selectionOperator)
                {
                    selectionOperator = value;
                    if(SelectionOperatorChanged != null)
                    {
                        SelectionOperatorChanged.Invoke(this, selectionOperator);
                    }
                }
            }
        }

        public bool ShowPremiumAnatomy
        {
            get
            {
                return showPremiumAnatomy;
            }
            set
            {
                if (showPremiumAnatomy != value)
                {
                    showPremiumAnatomy = value;
                    if (ShowPremiumAnatomyChanged != null)
                    {
                        ShowPremiumAnatomyChanged.Invoke(this, showPremiumAnatomy);
                    }
                }
            }
        }

        public AnatomyFilterEntry TopLevelMode
        {
            get
            {
                return currentTopLevelMode;
            }
            set
            {
                currentTopLevelMode = value;
            }
        }

        public void setCommandPermission(AnatomyCommandPermissions permission, bool enabled)
        {
            if (enabled)
            {
                CommandPermissions |= permission;
            }
            else
            {
                CommandPermissions &= (~permission);
            }
        }

        public void processSelection(Anatomy anatomy, Anatomy previousAnatomy)
        {
            switch (SelectionOperator)
            {
                case SelectionOperator.Select:
                    SelectedAnatomy.setSelection(anatomy);
                    break;
                case SelectionOperator.Add:
                    SelectedAnatomy.addSelection(anatomy);
                    if (previousAnatomy != null)
                    {
                        SelectedAnatomy.removeSelection(previousAnatomy);
                    }
                    break;
                case SelectionOperator.Remove:
                    SelectedAnatomy.removeSelection(anatomy);
                    break;
            }
        }

        public AnatomyCommandPermissions CommandPermissions
        {
            get
            {
                return commandPermissions;
            }
            private set
            {
                if (commandPermissions != value)
                {
                    commandPermissions = value;
                    if(CommandPermissionsChanged != null)
                    {
                        CommandPermissionsChanged.Invoke(this, commandPermissions);
                    }
                }
            }
        }

        public AnatomySelection SelectedAnatomy
        {
            get
            {
                return selectedAnatomy;
            }
        }

        public IEnumerable<AnatomyFilterEntry> FilterEntries
        {
            get
            {
                return luceneSearch.FilterEntries;
            }
        }

        /// <summary>
        /// Get the bounding box around all currently visible objects for the currently
        /// active camera. This is computed each time this function is called and enumerates
        /// all anatomy in the scene.
        /// </summary>
        public AxisAlignedBox VisibleObjectsBoundingBox
        {
            get
            {
                AxisAlignedBox box = AxisAlignedBox.MinMaxable;
                bool foundSomething = false;
                foreach (Anatomy anatomy in AnatomyManager.AnatomyList)
                {
                    if (anatomy.CurrentAlpha > 0.0f)
                    {
                        box.merge(anatomy.WorldBoundingBox);
                        foundSomething = true;
                    }
                }
                if (foundSomething)
                {
                    return box;
                }
                else
                {
                    return new AxisAlignedBox(); //Return a box with 0 bounds
                }
            }
        }

        private void fireDisplayAnatomy(Anatomy anatomy)
        {
            if (DisplayAnatomy != null)
            {
                DisplayAnatomy.Invoke(anatomy);
            }
        }

        private void fireClearDisplayedAnatomy()
        {
            if (ClearDisplayedAnatomy != null)
            {
                ClearDisplayedAnatomy.Invoke();
            }
        }

        private void fireSearchStarted(SuggestedDisplaySortMode suggestedSortMode)
        {
            if (SearchStarted != null)
            {
                SearchStarted.Invoke(suggestedSortMode);
            }
        }

        private void fireSearchEnded()
        {
            if (SearchEnded != null)
            {
                SearchEnded.Invoke();
            }
        }

        private void fireSuggestSearchCaption(String caption)
        {
            if(SuggestSearchCaption != null)
            {
                SuggestSearchCaption.Invoke(caption);
            }
        }

        private IEnumerable<Anatomy> currentClickGroupSelectionFor(SortedAnatomyClickResults matches)
        {
            if (showPremiumAnatomy)
            {
                Anatomy groupSelection = null;
                AnatomyGroup precursor;

                switch (PickingMode)
                {
                    case AnatomyPickingMode.Group:
                        //Start with group and go to individual
                        try
                        {
                            groupSelection = currentTopLevelMode.buildGroupSelectionFor(matches.Closest);
                        }
                        catch (Exception) { } //Ignore any exceptions.
                        if(groupSelection != null)
                        {
                            yield return groupSelection;
                        }

                        if(matches.Closest.IndividualSelectionPrecursor != null
                             && luceneSearch.tryGetGroup(matches.Closest.IndividualSelectionPrecursor, out precursor))
                        {
                            yield return precursor;
                        }

                        yield return matches.Closest;
                        break;
                    case AnatomyPickingMode.Individual:
                        //Go from individual to group
                        yield return matches.Closest;

                        if (matches.Closest.IndividualSelectionPrecursor != null
                             && luceneSearch.tryGetGroup(matches.Closest.IndividualSelectionPrecursor, out precursor))
                        {
                            yield return precursor;
                        }

                        try
                        {
                            groupSelection = currentTopLevelMode.buildGroupSelectionFor(matches.Closest);
                        }
                        catch (Exception) { } //Ignore any exceptions.
                        if (groupSelection != null)
                        {
                            yield return groupSelection;
                        }

                        break;
                }
            }
            else
            {
                AnatomyGroup groupSelection;
                luceneSearch.tryGetGroup(matches.Closest.Systems.FirstOrDefault(), out groupSelection);
                yield return groupSelection;
            }
        }

        void selectedAnatomy_DisplaySelectedAnatomy(IEnumerable<Anatomy> items)
        {
            displayAnatomy("Selected", items, SuggestedDisplaySortMode.Alphabetical);
        }
    }
}
