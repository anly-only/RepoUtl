using System.Diagnostics;

namespace RepoUtl
{
    internal static class UnitTests
    {
        internal static void Execute()
        {
            TestMergeInfo();
        }

        static void TestMergeInfo()
        {
            Debug.Assert(SvnMergeInfo.MinimizeRevisions("") == string.Empty);
            Debug.Assert(SvnMergeInfo.MinimizeRevisions("123") == "123");
            Debug.Assert(SvnMergeInfo.MinimizeRevisions("123,124") == "123-124");
            Debug.Assert(SvnMergeInfo.MinimizeRevisions("123,124,125") == "123-125");
            Debug.Assert(SvnMergeInfo.MinimizeRevisions("123-125,126") == "123-126");
            Debug.Assert(SvnMergeInfo.MinimizeRevisions("123-125,126-128") == "123-128");
            Debug.Assert(SvnMergeInfo.MinimizeRevisions("123-125,126,127-128") == "123-128");
            Debug.Assert(SvnMergeInfo.MinimizeRevisions("123-125,127,128-129") == "123-125,127-129");
            Debug.Assert(SvnMergeInfo.MinimizeRevisions("123-125,127,129-130") == "123-125,127,129-130");

            string a =
@"/_archive_/Branches/Feature PLCNENG 2020_0:94955-99060
/branches/AF30 Release:1-15228
/branches/AF30 SP Develop:1-15229
/branches/AF31 Release:1-33016,34063
/branches/AF31 SP Develop:1-33744,34065
/branches/AF310 SP Develop:67106
/branches/AF311 Release:69147-69148,69157-69158,69166,69173,69178,69186,69195,69199,69203,69209,69213,69218,69225,69229,69238-69239,69243,69245,69251,69257,69259-69261,69268,69273,69277-69278,69280,69285,69301,69304,69306,69314,69318,69322,69330,69337,69339,69346-69347,69356,69361,69366,69370,69374,69378,69383,69387,69390,69396,69401,69405,69410,69418,69420,69426,69431,69436-69438,69446,69452,69455-69456,69463,69467,69470,69478,69483,69488,69492,69496,69501,69507,69511,69516,69526,69531,69542,69546,69550,69556,69559,69562,69568,69574,69578,69582-69583,69600,69604,69611,69615,69624,69631,69635,69644,69646,69648,69651,69659,69663,69666-69667,69677-69678,69684,69688,69693,69698,69704-69705,69709-69710,69714,69718-69721,69726,69730-69731,69733,69743,69745,69751,69757-69758,69762,69774,69778,69782,69791,69796-69797,69800,69804,69809,69811,69813,69815,69820-69821,69831,69837,69854,69859,69863,69865,69872,69875,69885,69890,69896,69902,69906,69925,69929,69933,69940,69945,69949,69962,69970,69975,69977,69982,69997,70007-70008,70046,70059,70106,70130,70136,70198-70199,70385,70387,70395,70454
/branches/AF311 Release Protection:69991";
            string b = SvnMergeInfo.MinimizeMergeInfo(a);
            Debug.Assert(a == b);

            a =
@"/_archive_/Branches/Feature PLCNENG 2020_0:94955-99060,99061
/branches/AF30 Release:1-15228,15229";
            string x =
@"/_archive_/Branches/Feature PLCNENG 2020_0:94955-99061
/branches/AF30 Release:1-15229";
            b = SvnMergeInfo.MinimizeMergeInfo(a);
            Debug.Assert(x == b);
        }
    }
}
