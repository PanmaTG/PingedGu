// Password eye reveal
// class="toggle-password" -> Views/Authentication/Login.cshtml & Views/Authentication/Register.cshtml
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.toggle-password').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var input = document.getElementById(btn.dataset.target);
            var icon = btn.querySelector('ion-icon');

            if (input.type === 'password') {
                input.type = 'text';
                icon.setAttribute('name', 'eye-off-outline');
            } else {
                input.type = 'password';
                icon.setAttribute('name', 'eye-outline');
            }
        });
    });
});

//Resolves redirecting to page with no implemented tailwind css by clicking like or favorite button of posts in Favorites and My Profile page
// id="posts-container" -> Views/Users/Details.cshtml & Views/Favorites/Index.cshtml
document.addEventListener('DOMContentLoaded', function () {

    document.addEventListener('click', function (event) {
        const button = event.target.closest('.like-button, .favorite-button');
        if (!button) return;

        event.preventDefault();

        const form = button.closest('form');
        const postId = form.querySelector('input[name="postId"]').value;
        const postContainer = document.getElementById('post-' + postId);

        fetch(form.action, {
            method: 'POST',
            body: new FormData(form),
            headers: {}
        })
            .then(response => response.text())
            .then(html => {
                if (postContainer) postContainer.outerHTML = html;
            })
            .catch(error => console.error('Error: ', error));
    });

    document.addEventListener('submit', function (event) {
        const form = event.target;

        const ajaxFormClasses = [
            'add-comment-form',
            'remove-comment-form',
            'toggle-post-visibility-form',
            'remove-post-form',
            'add-post-report-form'
        ];

        const matchedClass = ajaxFormClasses.find(cls => form.classList.contains(cls));
        if (!matchedClass) return;

        event.preventDefault();

        const postId = form.querySelector('input[name="postId"]').value;
        const postContainer = document.getElementById('post-' + postId);

        fetch(form.action, {
            method: 'POST',
            body: new FormData(form),
            headers: {}
        })
            .then(response => response.text())
            .then(html => {
                if (postContainer) postContainer.outerHTML = html;
            })
            .catch(error => console.error('Error: ', error));
    });

});

// Reply Button disable/enable
// Views/Shared/Timeline/_Post.cshtml
document.addEventListener('input', function (e) {
    if (!e.target.classList.contains('reply-textarea')) return;

const form = e.target.closest('.add-comment-form');
const btn = form.querySelector('.reply-submit-btn');
    const hasContent = e.target.value.trim().length > 0;

btn.disabled = !hasContent;

btn.classList.toggle('bg-gray-200', !hasContent);
btn.classList.toggle('text-gray-400', !hasContent);
btn.classList.toggle('cursor-not-allowed', !hasContent);

btn.classList.toggle('bg-secondery', hasContent);
btn.classList.toggle('text-gray-700', hasContent);
btn.classList.toggle('hover:bg-gray-300', hasContent);
btn.classList.toggle('cursor-pointer', hasContent);
});
