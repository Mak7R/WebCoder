function fetchStructure(userName, title) {
    return fetch(`/repositories/${userName}/${title}/sources/structure`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            renderStructure(data);
        })
        .catch(error => {
            console.error('Error fetching data:', error);
        });
}

function renderStructure(structure) {
    const aside = document.getElementById('file-explorer-tree');
    aside.innerHTML = '';
    renderNode(structure, aside, '');
    afterRender();
}

function renderNode(node, parentElement, path) {
    const item = document.createElement('li');
    const itemName = document.createElement('span');
    const itemIcon = document.createElement('img')
    const  itemContent = document.createElement('span')

    itemIcon.classList.add('file-explorer-item-icon')
    
    
    itemName.textContent = node.name;

    let cur_path = path + '/' + node.name;
    
    itemContent.classList.add('file-explorer-item-content');
    
    itemContent.appendChild(itemIcon);
    itemContent.appendChild(itemName);

    itemContent.setAttribute('data-bs-toggle', 'popover');
    itemContent.setAttribute('data-bs-trigger', 'hover focus');
    itemContent.setAttribute('data-bs-content', cur_path);

    itemContent.addEventListener("click", function(event) {
        if (event.ctrlKey){
            let path = this.getAttribute('data-bs-content');

            navigator.clipboard.writeText(path).then(function() {
                
            }, function(err) {
                console.error("Error of copy value: ", err);
            });
        }
        
    });

    if (path === '' && node.type === 'Repository') {
        itemIcon.src = '/icons/icon-repository.png';
    } else if (node.type === 'Directory') {
        itemIcon.src = '/icons/icon-directory.svg';
    } else if (node.type === 'File') {
        itemIcon.src = '/icons/icon-file.svg';
        itemContent.addEventListener("click", function (event){
            if (!event.ctrlKey){
                let path = this.getAttribute('data-bs-content');
                requestFile(path);
            }
        });
    }
    
    new bootstrap.Popover(itemContent);
    
    item.classList.add('file-explorer-item');
    item.appendChild(itemContent);
    item.setAttribute('path', cur_path);

    

    // Рекурсивно обрабатываем дочерние элементы, если они есть
    if (node.children && node.children.length > 0) {
        itemContent.classList.add('caret')
        const childList = document.createElement('ul');
        childList.classList.add('nested')
        node.children.forEach(child => {
            renderNode(child, childList, cur_path);
        });
        item.appendChild(childList);
    }

    parentElement.appendChild(item);
}

function afterRender(){
    // init tree settings
    let togglers = document.getElementsByClassName("caret");
    for (let i = 0; i < togglers.length; i++) {
        togglers[i].addEventListener("click", function(event) {
            if (!event.ctrlKey){
                this.parentElement.querySelector(".nested").classList.toggle("active");
                this.classList.toggle("caret-down");
            }
        });
    }
}


async function requestFile(filePath) {
    const url = window.location.origin + window.location.pathname + '/get-file' + `?filePath=${filePath}`;

    await fetch(url, {
        method: "GET",
        headers: {
            "Content-Type": "text/plain"
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Network response was not ok " + response.statusText);
            }
            return response.text(); // Получаем текст из ответа
        })
        .then(data => {
            // Отображаем содержимое файла в textarea
            let pathElements = filePath.split('/');
            document.getElementById('file-name-view').innerText = pathElements[pathElements.length - 1];

            document.getElementById('file-path-edit-area').value = filePath;
            document.getElementById('file-edit-area').value = data;
        })
        .catch(error => {
            console.error("There was a problem with the fetch operation:", error);
        });
}