// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Android.Content;
using AndroidX.Fragment.App;
using Java.Lang;

namespace Contoso.Android.Puppet
{
    public class PagerAdapter : FragmentStatePagerAdapter
    {
        private readonly Context mContext;

        public PagerAdapter(FragmentManager fragmentManager, Context context) : base(fragmentManager)
        {
            mContext = context;
        }

        public override int Count
        {
            get { return 4; }
        }

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0: return new AppCenterFragment();
                case 1: return new AnalyticsFragment();
                case 2: return new CrashesFragment();
                case 3: return new OthersFragment();
                default: return null;
            }
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            switch (position)
            {
                case 0: return new String(mContext.GetString(Resource.String.AppCenterTitle));
                case 1: return new String(mContext.GetString(Resource.String.AnalyticsTitle));
                case 2: return new String(mContext.GetString(Resource.String.CrashesTitle));
                case 3: return new String(mContext.GetString(Resource.String.OthersTitle));
                default: return null;
            }
        }
    }
}
