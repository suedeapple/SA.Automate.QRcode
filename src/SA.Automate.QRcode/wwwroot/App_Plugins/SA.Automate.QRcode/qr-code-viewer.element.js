import { css, html, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';

/**
 * Read-only viewer for a QR code stored as a data URI, raw base64 PNG, or SVG markup. Offers a
 * Remove button to clear the value, but no way to set or type one — that's done by an Automate
 * workflow (Generate QR Code -> Update Content Property).
 */
export class SaQrCodeViewerElement extends UmbLitElement {
	static properties = {
		value: { type: String },
	};

	async #onRemove() {
		try {
			await umbConfirmModal(this, {
				headline: 'Remove QR code',
				content: 'Are you sure you want to remove this QR code?',
				color: 'danger',
				confirmLabel: 'Remove',
			});
		} catch {
			return; // user cancelled
		}

		this.value = undefined;
		this.dispatchEvent(new UmbChangeEvent());
	}

	#renderImage() {
		const trimmed = this.value.trim();

		if (trimmed.startsWith('<svg')) {
			return html`<div class="qr-svg">${unsafeHTML(trimmed)}</div>`;
		}

		const src = trimmed.startsWith('data:') ? trimmed : `data:image/png;base64,${trimmed}`;
		return html`<img src=${src} alt="QR code" />`;
	}

	render() {
		if (!this.value) {
			return html`<div class="empty">No QR code generated.</div>`;		
		}

		return html`
			<div class="qr-image">
				${this.#renderImage()}
				<uui-action-bar class="actions">
					<uui-button label="Remove" look="secondary" @click=${() => this.#onRemove()}>
						<uui-icon name="icon-trash"></uui-icon>
					</uui-button>
				</uui-action-bar>
			</div>
		`;
	}

	static styles = css`
		.qr-image {
			position: relative;
			display: inline-block;
		}

		.qr-image .actions {
			position: absolute;
			top: var(--uui-size-space-2);
			right: var(--uui-size-space-2);
			opacity: 0;
			transition: opacity 120ms ease-in-out;
		}

		.qr-image:hover .actions,
		.qr-image:focus-within .actions {
			opacity: 1;
		}

		img,
		.qr-svg svg {
			display: block;
			max-width: 200px;
			max-height: 200px;
		}

		.empty {
			color: var(--uui-color-text-alt);
			font-style: italic;
		}
	`;
}

customElements.define('sa-qr-code-viewer', SaQrCodeViewerElement);

export default SaQrCodeViewerElement;
