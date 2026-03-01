import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AuthService } from './core/services/auth.service';
import { CategoryService } from './core/services/category.service';
import { CategoryWithChildren } from './core/models';
import { LanguageSelectorComponent } from './shared/components/language-selector/language-selector.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, TranslateModule, LanguageSelectorComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  isScrolled = false;
  showCategories = false;
  showUserMenu = false;
  mobileMenuOpen = false;
  categoryTree: CategoryWithChildren[] = [];

  private readonly categoryIcons: Record<string, string> = {
    'automobiles': '🚗', 'motos': '🏍️', 'vehicules-utilitaires': '🚛', 'bateaux': '⛵',
    'immobilier': '🏠', 'informatique': '💻', 'telephonie': '📱', 'image-son': '📺',
    'electromenager': '🏪', 'ameublement': '🛋️', 'jardinage-bricolage': '🔧', 'sport-loisirs': '⚽',
    'vetements-accessoires': '👔', 'beaute-bien-etre': '💄', 'livres-musique-films': '📚',
    'jeux-jouets': '🎮', 'animaux': '🐾', 'emploi': '💼', 'services': '🤝',
    'cours-formations': '🎓', 'billets-evenements': '🎫', 'collections-art': '🎨',
    'montres-bijoux': '⌚', 'instruments-musique': '🎵', 'materiel-professionnel': '🏗️',
    'armes': '🎯', 'vins-gastronomie': '🍷', 'enfants-bebe': '👶', 'sante': '❤️',
    'voyages-vacances': '✈️', 'agriculture': '🌾', 'divers': '📦'
  };

  constructor(
    public authService: AuthService,
    private translate: TranslateService,
    private categoryService: CategoryService
  ) {
    this.translate.setDefaultLang('en');
    const saved = typeof localStorage !== 'undefined' ? localStorage.getItem('lang') : null;
    this.translate.use(saved || 'en');
  }

  ngOnInit(): void {
    this.categoryService.getTree().subscribe(cats => this.categoryTree = cats);
  }

  @HostListener('window:scroll')
  onScroll(): void {
    this.isScrolled = (typeof window !== 'undefined') && window.scrollY > 10;
  }

  toggleCategories(): void {
    this.showCategories = !this.showCategories;
    this.showUserMenu = false;
  }

  toggleUserMenu(): void {
    this.showUserMenu = !this.showUserMenu;
    this.showCategories = false;
  }

  getCategoryIcon(slug: string): string {
    return this.categoryIcons[slug] || '📂';
  }
}
