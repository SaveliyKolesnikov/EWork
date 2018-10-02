$(document).ready(function () {
    const searchInput = document.querySelector('.popup-hints-wrapper input');
    const searchResult = document.querySelector('.popup-hints-wrapper .hints');

    function loadHints(e) {
        if (!searchInput.value.length) {
            searchResult.style.visibility = 'hidden';
            return;
        }


        const token = $('input[name="__RequestVerificationToken"]').first().val();
        const lastEnteredTag = searchInput.value.slice(searchInput.value.lastIndexOf(' ') + 1);

        $.post('/Tag/GetTagsByFirstLetters',
            {
                '__RequestVerificationToken': token,
                'tagVal': lastEnteredTag
            },
            function (tags) {
                searchResult.style.visibility = 'visible';
                if (!tags.length) {
                    searchResult.innerHTML =
                        '<p align="center" style="width: 100%; margin: 0; padding-top: 10px; padding-bottom: 10px;">No results</p>';
                    return;
                }

                searchResult.innerHTML = '';
                for (const tag of tags) {
                    searchResult.innerHTML +=
                        `<p class="hint">${tag}</p>`;
                }

                $('.hint', searchResult).on('click', (e) => {
                    e.preventDefault();
                    addTagToInput(e.target.innerHTML);
                    searchResult.style.visibility = "hidden";
                });
            });
    }

    function addTagToInput(tagVal) {
        const lastTagStartIndex = searchInput.value.lastIndexOf(' ') + 1;
        const lastEnteredTag = searchInput.value.slice(lastTagStartIndex);
        if (tagVal.toLowerCase().indexOf(lastEnteredTag.toLowerCase()) !== 0)
            return;

        searchInput.value = searchInput.value.slice(0, lastTagStartIndex) + tagVal + ' ';
    }

    searchInput.addEventListener('input', loadHints);

    searchInput.addEventListener('focus', function() {
        if (this.value.length) {
            loadHints();
        }
    });

    searchResult.addEventListener("mouseleave", function (e) {
        if (searchInput !== document.activeElement) {
            searchResult.style.visibility = "hidden";
        }
    });

    searchResult.addEventListener("mouseover", function (e) {
        searchInput.removeEventListener("blur", searchInputBlur);
    });

    searchResult.addEventListener("mouseleave", function (e) {
        searchInput.addEventListener("blur", searchInputBlur);
    });

    searchResult.addEventListener('focus', function (e) {
        const state = searchInput.value !== "" ? "visible" : "hidden";
        searchResult.style.visibility = state;
    });

    function searchInputBlur() {
        searchResult.style.visibility = 'hidden';
    }
});