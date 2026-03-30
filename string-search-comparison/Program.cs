using string_search_comparison.Algorithms;

const string Text =
    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis maximus bibendum libero at viverra. Proin tempus convallis ante, at tincidunt urna tempor at. Quisque aliquet, dolor id fringilla auctor, lorem justo maximus mi, eu aliquet urna eros ac elit. Vestibulum placerat elementum leo eu tincidunt. Nunc sit amet ligula vehicula, scelerisque orci et, sodales arcu. Nullam vulputate a augue molestie ullamcorper. In ut mauris luctus, lacinia elit eget, suscipit eros. Aliquam efficitur leo ac lacus condimentum dictum. Fusce quis odio a dolor elementum gravida sed vitae nulla. Nam at ipsum sit amet sapien ultrices accumsan. Mauris posuere leo bibendum, dapibus urna at, maximus massa. Nullam id ipsum sapien. In hac habitasse platea dictumst.";

const string SearchTerm = "consectetur";

var naiveSearch = new NaiveSearch();
naiveSearch.Search(Text, SearchTerm);

var kmpSearch = new KmpSearch();
kmpSearch.Search(Text, SearchTerm);