// Password eye reveal
// class="toggle-password" -> Views/Authentication/Login.cshtml & Views/Authentication/Register.cshtml
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.toggle-password').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var input = document.getElementById(btn.dataset.target);
            var icon = btn.querySelector('ion-icon');

            icon.style.transition = 'transform 0.15s ease';

            if (input.type === 'password') {
                input.type = 'text';
                icon.setAttribute('name', 'eye-off-outline');
                icon.style.transform = 'scale(1.2)';
            } else {
                input.type = 'password';
                icon.setAttribute('name', 'eye-outline');
                icon.style.transform = 'scale(1)';
            }
        });
    });
});

//Resolves redirecting to page with no implemented tailwind css by clicking like or favorite button of posts in Favorites and My Profile page
// id="posts-container" -> Views/Users/Details.cshtml & Views/Favorites/Index.cshtml
document.addEventListener('click', function (event) {
    const button = event.target.closest('.like-button, .favorite-button');
    if (!button) return;

    event.preventDefault();

    if (button.disabled || button.classList.contains('is-processing')) return; 
    button.classList.add('is-processing');
    button.disabled = true; 

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
        .catch(error => {
            console.error('Error: ', error);
            button.disabled = false;
            button.classList.remove('is-processing');
        });
});

// Post submit button
// Views/Shared/Shared/StoryModals/_CreatePost.cshtml
document.querySelector('.create-post-form').addEventListener('submit', function (e) {
    const content = document.getElementById('postContent').value.trim();
    const warning = document.getElementById('contentWarning');

    if (content === '') {
        e.preventDefault();
        warning.classList.remove('hidden');
        return;
    }

    warning.classList.add('hidden');

    const submitBtn = this.querySelector('button[type="submit"]');
    submitBtn.disabled = true;
    submitBtn.textContent = 'Posting...';
    submitBtn.classList.remove('bg-blue-600');
    submitBtn.classList.add('bg-blue-400', 'cursor-not-allowed');
});

// Post submit button
// Views/Shared/Shared/StoryModals/_CreateStory.cshtml
document.querySelector('.create-story-form').addEventListener('submit', function (e) {
    const fileInput = document.getElementById('storyUpload');
    const warning = document.getElementById('storyWarning');

    if (!fileInput.files || fileInput.files.length === 0) {
        e.preventDefault();
        warning.classList.remove('hidden');
        return;
    }

    warning.classList.add('hidden');

    const submitBtn = this.querySelector('button[type="submit"]');
    submitBtn.disabled = true;
    submitBtn.textContent = 'Posting...';
    submitBtn.classList.remove('bg-blue-600');
    submitBtn.classList.add('bg-blue-400', 'cursor-not-allowed');
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
